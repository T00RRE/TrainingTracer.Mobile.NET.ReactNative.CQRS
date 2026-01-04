using MediatR;
using FluentValidation;
using Microsoft.EntityFrameworkCore;
using TrainingTracker.Client.Server.Data;
using TrainingTracker.Client.Server.Models;
using TrainingTracker.Client.Server.DTOs.Sessions;

namespace TrainingTracker.Client.Server.Features.Sessions
{
    // 1. COMMAND (Żądanie): Zwraca ID nowo rozpoczętej sesji
    public record StartTrainingSessionCommand(StartSessionDto Data) : IRequest<int>;


    // 2. WALIDATOR: Sprawdza istnienie UserId (klucz obcy)
    public class StartTrainingSessionValidator : AbstractValidator<StartTrainingSessionCommand>
    {
        private readonly ApplicationDbContext _context;

        public StartTrainingSessionValidator(ApplicationDbContext context)
        {
            _context = context;

            RuleFor(x => x.Data.UserId)
                .GreaterThan(0).WithMessage("ID użytkownika jest wymagane.")
                .MustAsync(UserMustExist).WithMessage("Użytkownik o podanym ID nie istnieje.");

            RuleFor(x => x.Data.TemplateId)
                .GreaterThan(0).WithMessage("ID szablonu jest wymagane.")
                .MustAsync(TemplateMustExist).WithMessage("Wybrany szablon treningowy nie istnieje.");
        }

        // Metoda walidująca klucz obcy
        private async Task<bool> UserMustExist(StartTrainingSessionCommand command, int userId, CancellationToken token)
        {
            if (userId <= 0) return false;
            return await _context.Users.AnyAsync(u => u.Id == userId, token);
        }
        private async Task<bool> TemplateMustExist(StartTrainingSessionCommand command, int templateId, CancellationToken token)
        {
            return await _context.WorkoutTemplates.AnyAsync(t => t.Id == templateId, token);
        }
    }


    // 3. HANDLER (Obsługa Logiki)
    public class StartTrainingSessionHandler : IRequestHandler<StartTrainingSessionCommand, int>
    {
        private readonly ApplicationDbContext _context;

        public StartTrainingSessionHandler(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<int> Handle(StartTrainingSessionCommand request, CancellationToken cancellationToken)
        {
            // 1. Rozpoczynamy transakcję, aby mieć pewność, że sesja i ćwiczenia zapiszą się razem
            using var transaction = await _context.Database.BeginTransactionAsync(cancellationToken);

            try
            {
                // 2. Tworzymy nową sesję (nagłówek)
                var newSession = new TrainingSession
                {
                    UserId = request.Data.UserId,
                    StartedAt = DateTime.UtcNow,
                    Notes = ""
                };

                _context.TrainingSessions.Add(newSession);
                // Musimy zapisać zmiany teraz, aby wygenerować Id dla newSession
                await _context.SaveChangesAsync(cancellationToken);

                // 3. Pobieramy ćwiczenia z wybranego szablonu
                var templateExercises = await _context.WorkoutTemplates
                    .Where(t => t.Id == request.Data.TemplateId)
                    .SelectMany(t => t.TemplateExercises) // Zakładając, że masz taką relację w modelu
                    .ToListAsync(cancellationToken);

                // Jeśli Twoja struktura bazy nie ma bezpośredniej relacji nawigacyjnej, użyj:
                // var templateExercises = await _context.TemplateExercises
                //    .Where(te => te.TemplateId == request.Data.TemplateId)
                //    .ToListAsync(cancellationToken);

                // 4. Kopiujemy ćwiczenia do nowej sesji
                foreach (var te in templateExercises)
                {
                    var sessionExercise = new SessionExercise
                    {
                        SessionId = newSession.Id,
                        ExerciseId = te.ExerciseId,
                    };
                    _context.SessionExercises.Add(sessionExercise);
                }

                await _context.SaveChangesAsync(cancellationToken);

                // 5. Zatwierdzamy transakcję
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

