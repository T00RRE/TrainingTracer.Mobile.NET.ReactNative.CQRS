using MediatR;
using FluentValidation;
using Microsoft.EntityFrameworkCore;
using TrainingTracker.Client.Server.Data;

namespace TrainingTracker.Client.Server.Features.SessionExercises
{
    // 1. COMMAND
    public record DeleteSessionExerciseCommand(int Id) : IRequest<Unit>;

    // 2. WALIDATOR
    public class DeleteSessionExerciseValidator : AbstractValidator<DeleteSessionExerciseCommand>
    {
        private readonly ApplicationDbContext _context;

        public DeleteSessionExerciseValidator(ApplicationDbContext context)
        {
            _context = context;

            RuleFor(x => x.Id)
                .GreaterThan(0).WithMessage("ID ćwiczenia w sesji jest wymagane do usunięcia.")
                .MustAsync(SessionExerciseMustExist).WithMessage("Ćwiczenie w sesji o podanym ID nie zostało znalezione.");
        }

        private async Task<bool> SessionExerciseMustExist(int id, CancellationToken token)
        {
            return await _context.SessionExercises.AnyAsync(se => se.Id == id, token);
        }
    }

    // 3. HANDLER
    public class DeleteSessionExerciseHandler : IRequestHandler<DeleteSessionExerciseCommand, Unit>
    {
        private readonly ApplicationDbContext _context;

        public DeleteSessionExerciseHandler(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<Unit> Handle(DeleteSessionExerciseCommand request, CancellationToken cancellationToken)
        {
            var sessionExerciseToDelete = await _context.SessionExercises
                .FirstAsync(se => se.Id == request.Id, cancellationToken);

            _context.SessionExercises.Remove(sessionExerciseToDelete);
            // CASCADE DELETE: Usunięcie SessionExercise usunie powiązane ExerciseSets
            await _context.SaveChangesAsync(cancellationToken);

            return Unit.Value;
        }
    }
}