using MediatR;
using FluentValidation;
using TrainingTracker.Client.Server.Data;
using TrainingTracker.Client.Server.Models;
using TrainingTracker.Client.Server.DTOs.Categories;

namespace TrainingTracker.Client.Server.Features.Categories
{
    public record CreateExerciseCategoryCommand(CreateCategoryDto Data) : IRequest<int>;

    public class CreateExerciseCategoryValidator : AbstractValidator<CreateExerciseCategoryCommand>
    {
        public CreateExerciseCategoryValidator()
        {
            RuleFor(x => x.Data.Name)
                .NotEmpty().WithMessage("Nazwa kategorii jest wymagana.")
                .MaximumLength(50).WithMessage("Nazwa jest za długa.");
        }
    }

    public class CreateExerciseCategoryHandler : IRequestHandler<CreateExerciseCategoryCommand, int>
    {
        private readonly ApplicationDbContext _context;

        public CreateExerciseCategoryHandler(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<int> Handle(CreateExerciseCategoryCommand request, CancellationToken cancellationToken)
        {
            var newCategory = new ExerciseCategory
            {
                Name = request.Data.Name
            };

            _context.ExerciseCategories.Add(newCategory);
            await _context.SaveChangesAsync(cancellationToken);

            return newCategory.Id;
        }
    }
}