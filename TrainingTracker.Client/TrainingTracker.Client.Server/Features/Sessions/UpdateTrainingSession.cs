using MediatR;
using FluentValidation;
using Microsoft.EntityFrameworkCore;
using TrainingTracker.Client.Server.Data;
using TrainingTracker.Client.Server.DTOs.Sessions;
using TrainingTracker.Client.Server.Models;

namespace TrainingTracker.Client.Server.Features.Sessions
{
    // 1. COMMAND (Żądanie): Obejmuje ID i dane do aktualizacji. Zwraca 'Unit'.
    public record UpdateTrainingSessionCommand(int Id, UpdateSessionDto Data) : IRequest<Unit>;


    // 2. WALIDATOR
    public class UpdateTrainingSessionValidator : AbstractValidator<UpdateTrainingSessionCommand>
    {
        private readonly ApplicationDbContext _context;

        public UpdateTrainingSessionValidator(ApplicationDbContext context)
        {
            _context = context;

            RuleFor(x => x.Id)
                .GreaterThan(0).WithMessage("ID sesji jest wymagane do aktualizacji.")
                .MustAsync(SessionMustExist).WithMessage("Sesja o podanym ID nie została znaleziona.");
        }

        private async Task<bool> SessionMustExist(int id, CancellationToken token)
        {
            return await _context.TrainingSessions.AnyAsync(s => s.Id == id, token);
        }
    }


    // 3. HANDLER (Obsługa Logiki)
    public class UpdateTrainingSessionHandler : IRequestHandler<UpdateTrainingSessionCommand, Unit>
    {
        private readonly ApplicationDbContext _context;

        public UpdateTrainingSessionHandler(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<Unit> Handle(UpdateTrainingSessionCommand request, CancellationToken cancellationToken)
        {
            var sessionToUpdate = await _context.TrainingSessions
                .FirstAsync(s => s.Id == request.Id, cancellationToken);

            // Aktualizacja notatek (jeśli podano)
            if (request.Data.Notes != null)
            {
                sessionToUpdate.Notes = request.Data.Notes;
            }

            // Zakończenie sesji (jeśli EndSession jest true i sesja nie jest jeszcze zakończona)
            if (request.Data.EndSession && sessionToUpdate.CompletedAt == null)
            {
                sessionToUpdate.CompletedAt = DateTime.UtcNow;
            }
            // Jeśli CompletedAt jest już ustawione, a EndSession jest true, to po prostu ignorujemy, 
            // zakładając, że to nie jest błąd, tylko ponowna próba zakończenia.


            // Zapis zmian
            await _context.SaveChangesAsync(cancellationToken);

            return Unit.Value;
        }
    }
}