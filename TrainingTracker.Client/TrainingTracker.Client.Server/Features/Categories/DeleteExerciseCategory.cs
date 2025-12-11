using MediatR;
using FluentValidation;
using Microsoft.EntityFrameworkCore;
using TrainingTracker.Client.Server.Data;

namespace TrainingTracker.Client.Server.Features.Categories
{
    // 1. COMMAND
    public record DeleteExerciseCategoryCommand(int Id) : IRequest<Unit>;

    // 2. WALIDATOR
    public class DeleteExerciseCategoryValidator : AbstractValidator<DeleteExerciseCategoryCommand>
    {
        private readonly ApplicationDbContext _context;

        public DeleteExerciseCategoryValidator(ApplicationDbContext context)
        {
            _context = context;

            RuleFor(x => x.Id)
                .GreaterThan(0).WithMessage("ID kategorii jest wymagane do usunięcia.")
                .MustAsync(CategoryMustExist).WithMessage("Kategoria o podanym ID nie została znaleziona.");
        }

        private async Task<bool> CategoryMustExist(int id, CancellationToken token)
        {
            return await _context.ExerciseCategories.AnyAsync(c => c.Id == id, token);
        }
    }

    // 3. HANDLER
    public class DeleteExerciseCategoryHandler : IRequestHandler<DeleteExerciseCategoryCommand, Unit>
    {
        private readonly ApplicationDbContext _context;

        public DeleteExerciseCategoryHandler(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<Unit> Handle(DeleteExerciseCategoryCommand request, CancellationToken cancellationToken)
        {
            var categoryToDelete = await _context.ExerciseCategories
                .FirstAsync(c => c.Id == request.Id, cancellationToken);

            _context.ExerciseCategories.Remove(categoryToDelete);
            // CASCADE DELETE: Usunięcie kategorii usunie również powiązane ćwiczenia.
            await _context.SaveChangesAsync(cancellationToken);

            return Unit.Value;
        }
    }
}