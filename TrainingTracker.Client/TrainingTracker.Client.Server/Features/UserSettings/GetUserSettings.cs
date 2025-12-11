using MediatR;
using Microsoft.EntityFrameworkCore;
using TrainingTracker.Client.Server.Data;
using TrainingTracker.Client.Server.DTOs.UserSettings;

namespace TrainingTracker.Client.Server.Features.UserSettings
{
    // 1. QUERY: Pobiera ustawienia dla danego UserId
    public record GetUserSettingsQuery(int UserId) : IRequest<UserSettingsDto>;

    // 2. HANDLER
    public class GetUserSettingsHandler : IRequestHandler<GetUserSettingsQuery, UserSettingsDto>
    {
        private readonly ApplicationDbContext _context;

        public GetUserSettingsHandler(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<UserSettingsDto> Handle(GetUserSettingsQuery request, CancellationToken cancellationToken)
        {
            var settings = await _context.UserSettings
                .Where(s => s.UserId == request.UserId)
                .Select(s => new UserSettingsDto
                {
                    Id = s.Id,
                    UserId = s.UserId,
                    WeightUnit = s.WeightUnit,
                    Theme = s.Theme
                })
                .FirstOrDefaultAsync(cancellationToken);

            return settings;
        }
    }
}