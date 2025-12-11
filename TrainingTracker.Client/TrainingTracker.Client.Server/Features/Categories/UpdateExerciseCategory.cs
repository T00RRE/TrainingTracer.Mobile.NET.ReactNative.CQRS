using MediatR;
using FluentValidation;
using Microsoft.EntityFrameworkCore;
using TrainingTracker.Client.Server.Data;
using TrainingTracker.Client.Server.DTOs.Categories;

namespace TrainingTracker.Client.Server.Features.Categories
{
    // 1. COMMAND: Używamy tego samego DTO do tworzenia i aktualizacji (tylko nazwa)
    public record UpdateExerciseCategoryCommand(int Id, CreateCategoryDto Data) : IRequest<Unit>;


    // 2. WALIDATOR
    public class UpdateExerciseCategoryValidator : AbstractValidator<UpdateExerciseCategoryCommand>
    {
        private readonly ApplicationDbContext _context;

        public UpdateExerciseCategoryValidator(ApplicationDbContext context)
        {
            _context = context;

            RuleFor(x => x.Id)
                .GreaterThan(0).WithMessage("ID kategorii jest wymagane do aktualizacji.")
                .MustAsync(CategoryMustExist).WithMessage("Kategoria o podanym ID nie została znaleziona.");

            RuleFor(x => x.Data.Name)
                .NotEmpty().WithMessage("Nazwa kategorii jest wymagana.")
                .MaximumLength(50).WithMessage("Nazwa jest za długa.");
        }

        private async Task<bool> CategoryMustExist(int id, CancellationToken token)
        {
            return await _context.ExerciseCategories.AnyAsync(c => c.Id == id, token);
        }
    }


    // 3. HANDLER
    public class UpdateExerciseCategoryHandler : IRequestHandler<UpdateExerciseCategoryCommand, Unit>
    {
        private readonly ApplicationDbContext _context;

        public UpdateExerciseCategoryHandler(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<Unit> Handle(UpdateExerciseCategoryCommand request, CancellationToken cancellationToken)
        {
            var categoryToUpdate = await _context.ExerciseCategories
                .FirstAsync(c => c.Id == request.Id, cancellationToken);

            categoryToUpdate.Name = request.Data.Name;

            await _context.SaveChangesAsync(cancellationToken);
            return Unit.Value;
        }
    }
}