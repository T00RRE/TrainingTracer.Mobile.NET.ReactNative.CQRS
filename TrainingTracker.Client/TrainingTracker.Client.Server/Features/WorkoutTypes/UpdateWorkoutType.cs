using MediatR;
using FluentValidation;
using Microsoft.EntityFrameworkCore;
using TrainingTracker.Client.Server.Data;
using TrainingTracker.Client.Server.DTOs.WorkoutTypes;

namespace TrainingTracker.Client.Server.Features.WorkoutTypes
{
    // 1. COMMAND
    public record UpdateWorkoutTypeCommand(int Id, CreateWorkoutTypeDto Data) : IRequest<Unit>;


    // 2. WALIDATOR
    public class UpdateWorkoutTypeValidator : AbstractValidator<UpdateWorkoutTypeCommand>
    {
        private readonly ApplicationDbContext _context;

        public UpdateWorkoutTypeValidator(ApplicationDbContext context)
        {
            _context = context;

            RuleFor(x => x.Id)
                .GreaterThan(0).WithMessage("ID typu treningu jest wymagane do aktualizacji.")
                .MustAsync(WorkoutTypeMustExist).WithMessage("Typ treningu o podanym ID nie został znaleziony.");

            RuleFor(x => x.Data.Name)
                .NotEmpty().WithMessage("Nazwa typu treningu jest wymagana.");
        }

        private async Task<bool> WorkoutTypeMustExist(int id, CancellationToken token)
        {
            return await _context.WorkoutTypes.AnyAsync(t => t.Id == id, token);
        }
    }


    // 3. HANDLER
    public class UpdateWorkoutTypeHandler : IRequestHandler<UpdateWorkoutTypeCommand, Unit>
    {
        private readonly ApplicationDbContext _context;

        public UpdateWorkoutTypeHandler(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<Unit> Handle(UpdateWorkoutTypeCommand request, CancellationToken cancellationToken)
        {
            var typeToUpdate = await _context.WorkoutTypes
                .FirstAsync(t => t.Id == request.Id, cancellationToken);

            typeToUpdate.Name = request.Data.Name;

            await _context.SaveChangesAsync(cancellationToken);
            return Unit.Value;
        }
    }
}