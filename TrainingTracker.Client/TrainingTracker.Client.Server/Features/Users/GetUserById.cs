using MediatR;
using Microsoft.EntityFrameworkCore;
using TrainingTracker.Client.Server.Data;
using TrainingTracker.Client.Server.DTOs.Users;

namespace TrainingTracker.Client.Server.Features.Users
{
    // 1. QUERY (Zapytanie): Zwraca UserDto
    public record GetUserByIdQuery(int Id) : IRequest<UserDto>;

    // 2. HANDLER (Obsługa Logiki)
    public class GetUserByIdHandler : IRequestHandler<GetUserByIdQuery, UserDto>
    {
        private readonly ApplicationDbContext _context;

        public GetUserByIdHandler(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<UserDto> Handle(GetUserByIdQuery request, CancellationToken cancellationToken)
        {
            var user = await _context.Users
                .Where(u => u.Id == request.Id)
                .Select(u => new UserDto
                {
                    Id = u.Id,
                    Username = u.Username,
                    Email = u.Email,
                    CreatedAt = u.CreatedAt
                })
                .FirstOrDefaultAsync(cancellationToken);

            // Zwracamy null, jeśli użytkownik nie istnieje, co spowoduje 404 w kontrolerze
            return user;
        }
    }
}