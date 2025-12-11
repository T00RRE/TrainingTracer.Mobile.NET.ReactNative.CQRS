using MediatR;
using FluentValidation;
using Microsoft.EntityFrameworkCore;
using TrainingTracker.Client.Server.Data;
using TrainingTracker.Client.Server.DTOs.Exercises;

namespace TrainingTracker.Client.Server.Features.Exercises
{
    // 1. COMMAND (Żądanie): Obejmuje ID zasobu i dane do aktualizacji. Zwraca 'Unit' (typ void dla MediatR).
    public record UpdateExerciseCommand(int Id, UpdateExerciseDto Data) : IRequest<Unit>;


    // 2. WALIDATOR: Sprawdza dane i upewnia się, że ID istnieje
    public class UpdateExerciseValidator : AbstractValidator<UpdateExerciseCommand>
    {
        private readonly ApplicationDbContext _context;

        public UpdateExerciseValidator(ApplicationDbContext context)
        {
            _context = context;

            RuleFor(x => x.Id)
                .GreaterThan(0).WithMessage("ID zasobu jest wymagane do aktualizacji.");

            RuleFor(x => x.Data.Name)
                .NotEmpty().WithMessage("Nazwa ćwiczenia jest wymagana.");

            RuleFor(x => x.Data.CategoryId)
                .GreaterThan(0).WithMessage("ID kategorii jest wymagane.")
                .MustAsync(BeValidCategory).WithMessage("Wybrana kategoria nie istnieje.");

            RuleFor(x => x.Id)
                .MustAsync(ExerciseMustExist).WithMessage("Ćwiczenie o podanym ID nie zostało znalezione.");
        }

        private async Task<bool> BeValidCategory(UpdateExerciseCommand command, int categoryId, CancellationToken token)
        {
            if (categoryId <= 0) return false;
            return await _context.ExerciseCategories.AnyAsync(c => c.Id == categoryId, token);
        }

        private async Task<bool> ExerciseMustExist(int id, CancellationToken token)
        {
            return await _context.Exercises.AnyAsync(e => e.Id == id, token);
        }
    }


    // 3. HANDLER (Obsługa Logiki)
    public class UpdateExerciseHandler : IRequestHandler<UpdateExerciseCommand, Unit>
    {
        private readonly ApplicationDbContext _context;

        public UpdateExerciseHandler(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<Unit> Handle(UpdateExerciseCommand request, CancellationToken cancellationToken)
        {
            // Znajdujemy ćwiczenie w bazie (Walidator zagwarantował, że istnieje)
            var exerciseToUpdate = await _context.Exercises
                .FirstAsync(e => e.Id == request.Id, cancellationToken);

            // Aktualizacja właściwości
            exerciseToUpdate.Name = request.Data.Name;
            exerciseToUpdate.Description = request.Data.Description;
            exerciseToUpdate.IsGlobal = request.Data.IsGlobal;
            exerciseToUpdate.CategoryId = request.Data.CategoryId;

            // Zapis zmian w bazie danych
            await _context.SaveChangesAsync(cancellationToken);

            // Zwracamy puste Unit, bo operacja Command nie zwraca danych
            return Unit.Value;
        }
    }
}