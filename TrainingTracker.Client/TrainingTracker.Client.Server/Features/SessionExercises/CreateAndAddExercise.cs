using MediatR;
using FluentValidation;
using Microsoft.EntityFrameworkCore;
using TrainingTracker.Client.Server.Data;
using TrainingTracker.Client.Server.Models;
using TrainingTracker.Client.Server.DTOs.SessionExercises;

namespace TrainingTracker.Client.Server.Features.SessionExercises
{
    // 1. COMMAND: Zwraca ID nowo utworzonego SessionExercise
    public record CreateAndAddExerciseCommand(CreateAndAddExerciseDto Data) : IRequest<int>;

    // 2. WALIDATOR
    public class CreateAndAddExerciseValidator : AbstractValidator<CreateAndAddExerciseCommand>
    {
        private readonly ApplicationDbContext _context;

        public CreateAndAddExerciseValidator(ApplicationDbContext context)
        {
            _context = context;

            RuleFor(x => x.Data.Name)
                .NotEmpty().WithMessage("Nazwa ćwiczenia jest wymagana.");

            RuleFor(x => x.Data.SessionId)
                .MustAsync(SessionMustExistAndBeActive).WithMessage("Sesja nie istnieje lub jest zakończona.");
        }

        private async Task<bool> SessionMustExistAndBeActive(int sessionId, CancellationToken token)
        {
            return await _context.TrainingSessions.AnyAsync(s => s.Id == sessionId && s.CompletedAt == null, token);
        }
    }

    // 3. HANDLER
    public class CreateAndAddExerciseHandler : IRequestHandler<CreateAndAddExerciseCommand, int>
    {
        private readonly ApplicationDbContext _context;

        public CreateAndAddExerciseHandler(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<int> Handle(CreateAndAddExerciseCommand request, CancellationToken cancellationToken)
        {
            // Operacja w transakcji, aby mieć pewność, że obie rzeczy się udadzą
            using var transaction = await _context.Database.BeginTransactionAsync(cancellationToken);

            try
            {
                // A. Tworzymy nowe ćwiczenie w tabeli Exercises (Opcja A)
                var newExercise = new Exercise
                {
                    Name = request.Data.Name,
                    Description = request.Data.Description,
                    CategoryId = 1, // Sztywne przypisanie kategorii
                    IsGlobal = false // Ćwiczenie stworzone przez użytkownika "w locie"
                };

                _context.Exercises.Add(newExercise);
                await _context.SaveChangesAsync(cancellationToken);

                // B. Pobieramy ostatnią pozycję w sesji, aby dodać nowe ćwiczenie na koniec
                var lastPosition = await _context.SessionExercises
                    .Where(se => se.SessionId == request.Data.SessionId)
                    .Select(se => (int?)se.OrderPosition)
                    .MaxAsync(cancellationToken) ?? 0;

                // C. Przypisujemy to ćwiczenie do aktualnej sesji
                var newSessionExercise = new SessionExercise
                {
                    SessionId = request.Data.SessionId,
                    ExerciseId = newExercise.Id,
                    OrderPosition = lastPosition + 1
                };

                _context.SessionExercises.Add(newSessionExercise);
                await _context.SaveChangesAsync(cancellationToken);

                await transaction.CommitAsync(cancellationToken);

                return newSessionExercise.Id;
            }
            catch (Exception)
            {
                await transaction.RollbackAsync(cancellationToken);
                throw;
            }
        }
    }
}