using MediatR;
using FluentValidation;
using Microsoft.EntityFrameworkCore;
using TrainingTracker.Client.Server.Data;

namespace TrainingTracker.Client.Server.Features.WorkoutTypes
{
    // 1. COMMAND
    public record DeleteWorkoutTypeCommand(int Id) : IRequest<Unit>;

    // 2. WALIDATOR
    public class DeleteWorkoutTypeValidator : AbstractValidator<DeleteWorkoutTypeCommand>
    {
        private readonly ApplicationDbContext _context;

        public DeleteWorkoutTypeValidator(ApplicationDbContext context)
        {
            _context = context;

            RuleFor(x => x.Id)
                .GreaterThan(0).WithMessage("ID typu treningu jest wymagane do usunięcia.")
                .MustAsync(WorkoutTypeMustExist).WithMessage("Typ treningu o podanym ID nie został znaleziony.");
        }

        private async Task<bool> WorkoutTypeMustExist(int id, CancellationToken token)
        {
            return await _context.WorkoutTypes.AnyAsync(t => t.Id == id, token);
        }
    }

    // 3. HANDLER
    public class DeleteWorkoutTypeHandler : IRequestHandler<DeleteWorkoutTypeCommand, Unit>
    {
        private readonly ApplicationDbContext _context;

        public DeleteWorkoutTypeHandler(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<Unit> Handle(DeleteWorkoutTypeCommand request, CancellationToken cancellationToken)
        {
            var typeToDelete = await _context.WorkoutTypes
                .FirstAsync(t => t.Id == request.Id, cancellationToken);

            _context.WorkoutTypes.Remove(typeToDelete);
            await _context.SaveChangesAsync(cancellationToken);

            return Unit.Value;
        }
    }
}