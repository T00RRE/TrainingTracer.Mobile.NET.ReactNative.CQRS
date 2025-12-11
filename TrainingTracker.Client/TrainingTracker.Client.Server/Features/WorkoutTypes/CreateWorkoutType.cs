using MediatR;
using FluentValidation;
using TrainingTracker.Client.Server.Data;
using TrainingTracker.Client.Server.Models;
using TrainingTracker.Client.Server.DTOs.WorkoutTypes;

namespace TrainingTracker.Client.Server.Features.WorkoutTypes
{
    public record CreateWorkoutTypeCommand(CreateWorkoutTypeDto Data) : IRequest<int>;

    public class CreateWorkoutTypeValidator : AbstractValidator<CreateWorkoutTypeCommand>
    {
        public CreateWorkoutTypeValidator()
        {
            RuleFor(x => x.Data.Name)
                .NotEmpty().WithMessage("Nazwa typu treningu jest wymagana.")
                .MaximumLength(50).WithMessage("Nazwa jest za długa.");
        }
    }

    public class CreateWorkoutTypeHandler : IRequestHandler<CreateWorkoutTypeCommand, int>
    {
        private readonly ApplicationDbContext _context;

        public CreateWorkoutTypeHandler(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<int> Handle(CreateWorkoutTypeCommand request, CancellationToken cancellationToken)
        {
            var newType = new WorkoutType
            {
                Name = request.Data.Name
            };

            _context.WorkoutTypes.Add(newType);
            await _context.SaveChangesAsync(cancellationToken);

            return newType.Id;
        }
    }
}