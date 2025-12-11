using MediatR;
using FluentValidation;
using Microsoft.EntityFrameworkCore;
using TrainingTracker.Client.Server.Data;

namespace TrainingTracker.Client.Server.Features.Sessions
{
    // 1. COMMAND: Obejmuje ID sesji do usunięcia.
    public record DeleteTrainingSessionCommand(int Id) : IRequest<Unit>;

    // 2. WALIDATOR: Zapewnia, że sesja istnieje
    public class DeleteTrainingSessionValidator : AbstractValidator<DeleteTrainingSessionCommand>
    {
        private readonly ApplicationDbContext _context;

        public DeleteTrainingSessionValidator(ApplicationDbContext context)
        {
            _context = context;

            RuleFor(x => x.Id)
                .GreaterThan(0).WithMessage("ID sesji jest wymagane do usunięcia.")
                .MustAsync(SessionMustExist).WithMessage("Sesja o podanym ID nie została znaleziona.");
        }

        private async Task<bool> SessionMustExist(int id, CancellationToken token)
        {
            return await _context.TrainingSessions.AnyAsync(s => s.Id == id, token);
        }
    }

    // 3. HANDLER
    public class DeleteTrainingSessionHandler : IRequestHandler<DeleteTrainingSessionCommand, Unit>
    {
        private readonly ApplicationDbContext _context;

        public DeleteTrainingSessionHandler(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<Unit> Handle(DeleteTrainingSessionCommand request, CancellationToken cancellationToken)
        {
            var sessionToDelete = await _context.TrainingSessions
                .FirstAsync(s => s.Id == request.Id, cancellationToken);

            _context.TrainingSessions.Remove(sessionToDelete);
            // CASCADE DELETE: Usunięcie sesji usunie SessionExercises i ExerciseSets
            await _context.SaveChangesAsync(cancellationToken);

            return Unit.Value;
        }
    }
}