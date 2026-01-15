using MediatR;
using TrainingTracker.Client.Server.Data; 

namespace TrainingTracker.Client.Server.Features.Users
{
    public record AddUserCreditsCommand(int UserId, int Amount) : IRequest<int>;

    // 2. Handler
    public class AddUserCreditsHandler : IRequestHandler<AddUserCreditsCommand, int>
    {
        private readonly ApplicationDbContext _context;

        public AddUserCreditsHandler(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<int> Handle(AddUserCreditsCommand request, CancellationToken cancellationToken)
        {
            // Szukamy użytkownika
            var user = await _context.Users.FindAsync(request.UserId);

            // Jeśli nie ma takiego użytkownika, zwracamy -1 (błąd)
            if (user == null) return -1;

            // Dodajemy kredyty (np. +1, +3, +5)
            user.TrainingPlanCredits += request.Amount;

            // Zapisujemy zmiany w bazie
            await _context.SaveChangesAsync(cancellationToken);

            // Zwracamy aktualną liczbę kredytów
            return user.TrainingPlanCredits;
        }
    }
}