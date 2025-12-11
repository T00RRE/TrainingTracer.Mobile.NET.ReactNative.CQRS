using FluentValidation;
using MediatR;
using Microsoft.EntityFrameworkCore;
using TrainingTracker.Client.Server.Data;
using TrainingTracker.Client.Server.DTOs.Exercises;
using TrainingTracker.Client.Server.Models;

namespace TrainingTracker.Client.Server.Features.Exercises
{
    // 1. COMMAND (Żądanie): Zwraca Id utworzonego ćwiczenia
    public record CreateExerciseCommand(CreateExerciseDto Data) : IRequest<int>;

    // 2. WALIDATOR: Sprawdza dane przed wykonaniem Command
    public class CreateExerciseValidator : AbstractValidator<CreateExerciseCommand>
    {
        private readonly ApplicationDbContext _context;

        public CreateExerciseValidator(ApplicationDbContext context)
        {
            _context = context;

            RuleFor(x => x.Data.Name)
                .NotEmpty().WithMessage("Nazwa ćwiczenia jest wymagana.")
                .MaximumLength(100).WithMessage("Nazwa jest za długa.");

            RuleFor(x => x.Data.CategoryId)
                .GreaterThan(0).WithMessage("ID kategorii jest wymagane.")
                .MustAsync(BeValidCategory).WithMessage("Wybrana kategoria nie istnieje.");
        }

        // Metoda sprawdzająca, czy kategoria istnieje w bazie (na kluczu obcym)
        private async Task<bool> BeValidCategory(CreateExerciseCommand command, int categoryId, CancellationToken token)
        {
            if (categoryId <= 0) return false;
            return await _context.ExerciseCategories.AnyAsync(c => c.Id == categoryId, token);
        }
    }


    // 3. HANDLER (Obsługa Logiki)
    public class CreateExerciseHandler : IRequestHandler<CreateExerciseCommand, int>
    {
        private readonly ApplicationDbContext _context;

        public CreateExerciseHandler(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<int> Handle(CreateExerciseCommand request, CancellationToken cancellationToken)
        {
            // Tworzenie nowego modelu/encji
            var newExercise = new Exercise
            {
                Name = request.Data.Name,
                Description = request.Data.Description,
                IsGlobal = request.Data.IsGlobal,
                CategoryId = request.Data.CategoryId,
            };

            _context.Exercises.Add(newExercise);
            await _context.SaveChangesAsync(cancellationToken);

            // Zwracamy Id nowo utworzonego obiektu
            return newExercise.Id;
        }
    }
}