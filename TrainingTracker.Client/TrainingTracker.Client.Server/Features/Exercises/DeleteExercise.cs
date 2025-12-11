using MediatR;
using FluentValidation;
using Microsoft.EntityFrameworkCore;
using TrainingTracker.Client.Server.Data;

namespace TrainingTracker.Client.Server.Features.Exercises
{
    // 1. COMMAND (Żądanie): Akceptuje tylko ID zasobu do usunięcia. Zwraca 'Unit'.
    public record DeleteExerciseCommand(int Id) : IRequest<Unit>;


    // 2. WALIDATOR: Zapewnia, że zasób istnieje przed próbą usunięcia
    public class DeleteExerciseValidator : AbstractValidator<DeleteExerciseCommand>
    {
        private readonly ApplicationDbContext _context;

        public DeleteExerciseValidator(ApplicationDbContext context)
        {
            _context = context;

            RuleFor(x => x.Id)
                .GreaterThan(0).WithMessage("ID zasobu do usunięcia jest wymagane.")
                .MustAsync(ExerciseMustExist).WithMessage("Ćwiczenie o podanym ID nie zostało znalezione.");
        }

        // Metoda sprawdzająca, czy ćwiczenie o danym ID istnieje
        private async Task<bool> ExerciseMustExist(int id, CancellationToken token)
        {
            return await _context.Exercises.AnyAsync(e => e.Id == id, token);
        }
    }


    // 3. HANDLER (Obsługa Logiki)
    public class DeleteExerciseHandler : IRequestHandler<DeleteExerciseCommand, Unit>
    {
        private readonly ApplicationDbContext _context;

        public DeleteExerciseHandler(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<Unit> Handle(DeleteExerciseCommand request, CancellationToken cancellationToken)
        {
            // Znajdujemy encję do usunięcia (Walidator gwarantuje, że istnieje)
            var exerciseToDelete = await _context.Exercises
                .FirstAsync(e => e.Id == request.Id, cancellationToken);

            // Usuwanie encji
            _context.Exercises.Remove(exerciseToDelete);

            // Zapis zmian w bazie danych
            await _context.SaveChangesAsync(cancellationToken);

            // Zwracamy puste Unit, bo operacja Command nie zwraca danych
            return Unit.Value;
        }
    }
}