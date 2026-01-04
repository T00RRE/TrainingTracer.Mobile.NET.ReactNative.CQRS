using MediatR;
using FluentValidation;
using Microsoft.EntityFrameworkCore;
using TrainingTracker.Client.Server.Data;
using TrainingTracker.Client.Server.Models;
using TrainingTracker.Client.Server.DTOs.Sessions;

namespace TrainingTracker.Client.Server.Features.Sessions
{
    public record StartTrainingSessionCommand(StartSessionDto Data) : IRequest<int>;

    public class StartTrainingSessionHandler : IRequestHandler<StartTrainingSessionCommand, int>
    {
        private readonly ApplicationDbContext _context;

        public StartTrainingSessionHandler(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<int> Handle(StartTrainingSessionCommand request, CancellationToken cancellationToken)
        {
            // 1. SZUKAMY AKTYWNEJ SESJI DLA TEGO SAMEGO SZABLONU
            // Sprawdzamy, czy użytkownik ma sesję, która nie jest zakończona (CompletedAt == null)
            // i która została utworzona z tego samego szablonu.
            var existingActiveSession = await _context.TrainingSessions
                .FirstOrDefaultAsync(s => s.UserId == request.Data.UserId
                                       && s.TemplateId == request.Data.TemplateId
                                       && s.CompletedAt == null, cancellationToken);

            if (existingActiveSession != null)
            {
                // ZNALAZŁO: Zamiast tworzyć nową, zwracamy ID starej sesji.
                // Aplikacja mobilna po prostu otworzy ten sam ekran z tymi samymi danymi.
                return existingActiveSession.Id;
            }

            // 2. NIE ZNALAZŁO: Tworzymy nową sesję (Twoja stara logika)
            using var transaction = await _context.Database.BeginTransactionAsync(cancellationToken);

            try
            {
                var newSession = new TrainingSession
                {
                    UserId = request.Data.UserId,
                    TemplateId = request.Data.TemplateId, // Zapisujemy ID szablonu!
                    StartedAt = DateTime.UtcNow,
                    Notes = ""
                };

                _context.TrainingSessions.Add(newSession);
                await _context.SaveChangesAsync(cancellationToken);

                // Kopiujemy ćwiczenia z szablonu
                var templateExercises = await _context.WorkoutTemplates
                    .Where(t => t.Id == request.Data.TemplateId)
                    .SelectMany(t => t.TemplateExercises)
                    .ToListAsync(cancellationToken);

                foreach (var te in templateExercises)
                {
                    var sessionExercise = new SessionExercise
                    {
                        SessionId = newSession.Id,
                        ExerciseId = te.ExerciseId,
                        // OrderPosition = te.OrderPosition // Opcjonalnie, jeśli masz to pole
                    };
                    _context.SessionExercises.Add(sessionExercise);
                }

                await _context.SaveChangesAsync(cancellationToken);
                await transaction.CommitAsync(cancellationToken);

                return newSession.Id;
            }
            catch (Exception)
            {
                await transaction.RollbackAsync(cancellationToken);
                throw;
            }
        }
    }
}