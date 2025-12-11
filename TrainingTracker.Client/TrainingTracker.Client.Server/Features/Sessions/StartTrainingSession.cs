using MediatR;
using FluentValidation;
using Microsoft.EntityFrameworkCore;
using TrainingTracker.Client.Server.Data;
using TrainingTracker.Client.Server.Models;
using TrainingTracker.Client.Server.DTOs.Sessions;

namespace TrainingTracker.Client.Server.Features.Sessions
{
    // 1. COMMAND (Żądanie): Zwraca ID nowo rozpoczętej sesji
    public record StartTrainingSessionCommand(StartSessionDto Data) : IRequest<int>;


    // 2. WALIDATOR: Sprawdza istnienie UserId (klucz obcy)
    public class StartTrainingSessionValidator : AbstractValidator<StartTrainingSessionCommand>
    {
        private readonly ApplicationDbContext _context;

        public StartTrainingSessionValidator(ApplicationDbContext context)
        {
            _context = context;

            RuleFor(x => x.Data.UserId)
                .GreaterThan(0).WithMessage("ID użytkownika jest wymagane.")
                .MustAsync(UserMustExist).WithMessage("Użytkownik o podanym ID nie istnieje.");
        }

        // Metoda walidująca klucz obcy
        private async Task<bool> UserMustExist(StartTrainingSessionCommand command, int userId, CancellationToken token)
        {
            if (userId <= 0) return false;
            return await _context.Users.AnyAsync(u => u.Id == userId, token);
        }
    }


    // 3. HANDLER (Obsługa Logiki)
    public class StartTrainingSessionHandler : IRequestHandler<StartTrainingSessionCommand, int>
    {
        private readonly ApplicationDbContext _context;

        public StartTrainingSessionHandler(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<int> Handle(StartTrainingSessionCommand request, CancellationToken cancellationToken)
        {
            var newSession = new TrainingSession
            {
                UserId = request.Data.UserId,
                StartedAt = DateTime.UtcNow,
                Notes = "" // Domyślna wartość
                // CompletedAt jest domyślnie NULL (sesja trwa)
            };

            _context.TrainingSessions.Add(newSession);
            await _context.SaveChangesAsync(cancellationToken);

            return newSession.Id;
        }
    }
}