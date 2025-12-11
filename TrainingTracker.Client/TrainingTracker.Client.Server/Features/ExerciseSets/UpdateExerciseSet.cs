using MediatR;
using FluentValidation;
using Microsoft.EntityFrameworkCore;
using TrainingTracker.Client.Server.Data;
using TrainingTracker.Client.Server.DTOs.ExerciseSets;

namespace TrainingTracker.Client.Server.Features.ExerciseSets
{
    // 1. COMMAND
    public record UpdateExerciseSetCommand(int Id, UpdateSetDto Data) : IRequest<Unit>;

    // 2. WALIDATOR
    public class UpdateExerciseSetValidator : AbstractValidator<UpdateExerciseSetCommand>
    {
        private readonly ApplicationDbContext _context;

        public UpdateExerciseSetValidator(ApplicationDbContext context)
        {
            _context = context;

            RuleFor(x => x.Id)
                .GreaterThan(0).WithMessage("ID serii jest wymagane do aktualizacji.")
                .MustAsync(SetMustExist).WithMessage("Seria o podanym ID nie została znaleziona.");

            RuleFor(x => x.Data.Weight)
                .GreaterThanOrEqualTo(0).WithMessage("Waga musi być większa lub równa 0.");

            RuleFor(x => x.Data.Reps)
                .GreaterThanOrEqualTo(1).WithMessage("Liczba powtórzeń musi być co najmniej 1.");
        }

        private async Task<bool> SetMustExist(int id, CancellationToken token)
        {
            return await _context.ExerciseSets.AnyAsync(s => s.Id == id, token);
        }
    }

    // 3. HANDLER
    public class UpdateExerciseSetHandler : IRequestHandler<UpdateExerciseSetCommand, Unit>
    {
        private readonly ApplicationDbContext _context;

        public UpdateExerciseSetHandler(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<Unit> Handle(UpdateExerciseSetCommand request, CancellationToken cancellationToken)
        {
            var setToUpdate = await _context.ExerciseSets
                .FirstAsync(s => s.Id == request.Id, cancellationToken);

            setToUpdate.SetNumber = request.Data.SetNumber;
            setToUpdate.Weight = request.Data.Weight;
            setToUpdate.Reps = request.Data.Reps;

            // Uaktualniamy też czas ukończenia, aby zachować świeżość danych
            setToUpdate.CompletedAt = DateTime.UtcNow;

            await _context.SaveChangesAsync(cancellationToken);
            return Unit.Value;
        }
    }
}