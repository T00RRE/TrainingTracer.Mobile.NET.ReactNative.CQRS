using MediatR;
using FluentValidation;
using Microsoft.EntityFrameworkCore;
using TrainingTracker.Client.Server.Data;
using TrainingTracker.Client.Server.Models;
using TrainingTracker.Client.Server.DTOs.SessionExercises;

namespace TrainingTracker.Client.Server.Features.SessionExercises
{
    // 1. COMMAND
    public record AddSessionExerciseCommand(AddSessionExerciseDto Data) : IRequest<int>;


    // 2. WALIDATOR (Sprawdza klucze obce i czy sesja jest aktywna)
    public class AddSessionExerciseValidator : AbstractValidator<AddSessionExerciseCommand>
    {
        private readonly ApplicationDbContext _context;

        public AddSessionExerciseValidator(ApplicationDbContext context)
        {
            _context = context;

            RuleFor(x => x.Data.SessionId)
                .MustAsync(SessionMustExistAndBeActive).WithMessage("Sesja musi istnieć i być aktywna (nie zakończona).");

            RuleFor(x => x.Data.ExerciseId)
                .MustAsync(ExerciseMustExist).WithMessage("Ćwiczenie o podanym ID nie istnieje.");
        }

        private async Task<bool> SessionMustExistAndBeActive(AddSessionExerciseCommand command, int sessionId, CancellationToken token)
        {
            if (sessionId <= 0) return false;
            // Musi istnieć ORAZ CompletedAt musi być NULL (aktywna sesja)
            return await _context.TrainingSessions.AnyAsync(s => s.Id == sessionId && s.CompletedAt == null, token);
        }

        private async Task<bool> ExerciseMustExist(AddSessionExerciseCommand command, int exerciseId, CancellationToken token)
        {
            return await _context.Exercises.AnyAsync(e => e.Id == exerciseId, token);
        }
    }


    // 3. HANDLER
    public class AddSessionExerciseHandler : IRequestHandler<AddSessionExerciseCommand, int>
    {
        private readonly ApplicationDbContext _context;

        public AddSessionExerciseHandler(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<int> Handle(AddSessionExerciseCommand request, CancellationToken cancellationToken)
        {
            var newSessionExercise = new SessionExercise
            {
                SessionId = request.Data.SessionId,
                ExerciseId = request.Data.ExerciseId,
                OrderPosition = request.Data.OrderPosition
            };

            _context.SessionExercises.Add(newSessionExercise);
            await _context.SaveChangesAsync(cancellationToken);

            return newSessionExercise.Id;
        }
    }
}