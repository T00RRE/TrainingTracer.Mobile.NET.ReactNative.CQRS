using MediatR;
using FluentValidation;
using Microsoft.EntityFrameworkCore;
using TrainingTracker.Client.Server.Data;
using TrainingTracker.Client.Server.Models;
using TrainingTracker.Client.Server.DTOs.ExerciseSets;

namespace TrainingTracker.Client.Server.Features.ExerciseSets
{
    // 1. COMMAND
    public record AddExerciseSetCommand(AddSetDto Data) : IRequest<int>;


    // 2. WALIDATOR (Sprawdza klucz obcy)
    public class AddExerciseSetValidator : AbstractValidator<AddExerciseSetCommand>
    {
        private readonly ApplicationDbContext _context;

        public AddExerciseSetValidator(ApplicationDbContext context)
        {
            _context = context;

            RuleFor(x => x.Data.SessionExerciseId)
                .GreaterThan(0).WithMessage("ID ćwiczenia w sesji jest wymagane.")
                .MustAsync(SessionExerciseMustExist).WithMessage("Ćwiczenie w sesji o podanym ID nie istnieje.");

            RuleFor(x => x.Data.Weight)
                .GreaterThanOrEqualTo(0).WithMessage("Waga musi być większa lub równa 0.");

            RuleFor(x => x.Data.Reps)
                .GreaterThanOrEqualTo(1).WithMessage("Liczba powtórzeń musi być co najmniej 1.");
        }

        private async Task<bool> SessionExerciseMustExist(AddExerciseSetCommand command, int sessionExerciseId, CancellationToken token)
        {
            return await _context.SessionExercises.AnyAsync(se => se.Id == sessionExerciseId, token);
        }
    }


    // 3. HANDLER
    public class AddExerciseSetHandler : IRequestHandler<AddExerciseSetCommand, int>
    {
        private readonly ApplicationDbContext _context;

        public AddExerciseSetHandler(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<int> Handle(AddExerciseSetCommand request, CancellationToken cancellationToken)
        {
            var newSet = new ExerciseSet
            {
                SessionExerciseId = request.Data.SessionExerciseId,
                SetNumber = request.Data.SetNumber,
                Weight = request.Data.Weight,
                Reps = request.Data.Reps,
                CompletedAt = DateTime.UtcNow
            };

            _context.ExerciseSets.Add(newSet);
            await _context.SaveChangesAsync(cancellationToken);

            return newSet.Id;
        }
    }
}