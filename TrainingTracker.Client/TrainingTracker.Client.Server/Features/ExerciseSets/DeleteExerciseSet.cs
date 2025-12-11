using MediatR;
using FluentValidation;
using Microsoft.EntityFrameworkCore;
using TrainingTracker.Client.Server.Data;

namespace TrainingTracker.Client.Server.Features.ExerciseSets
{
    // 1. COMMAND
    public record DeleteExerciseSetCommand(int Id) : IRequest<Unit>;

    // 2. WALIDATOR
    public class DeleteExerciseSetValidator : AbstractValidator<DeleteExerciseSetCommand>
    {
        private readonly ApplicationDbContext _context;

        public DeleteExerciseSetValidator(ApplicationDbContext context)
        {
            _context = context;

            RuleFor(x => x.Id)
                .GreaterThan(0).WithMessage("ID serii jest wymagane do usunięcia.")
                .MustAsync(SetMustExist).WithMessage("Seria o podanym ID nie została znaleziona.");
        }

        private async Task<bool> SetMustExist(int id, CancellationToken token)
        {
            return await _context.ExerciseSets.AnyAsync(s => s.Id == id, token);
        }
    }

    // 3. HANDLER
    public class DeleteExerciseSetHandler : IRequestHandler<DeleteExerciseSetCommand, Unit>
    {
        private readonly ApplicationDbContext _context;

        public DeleteExerciseSetHandler(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<Unit> Handle(DeleteExerciseSetCommand request, CancellationToken cancellationToken)
        {
            var setToDelete = await _context.ExerciseSets
                .FirstAsync(s => s.Id == request.Id, cancellationToken);

            _context.ExerciseSets.Remove(setToDelete);
            await _context.SaveChangesAsync(cancellationToken);

            return Unit.Value;
        }
    }
}