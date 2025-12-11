using MediatR;
using Microsoft.EntityFrameworkCore;
using TrainingTracker.Client.Server.Data;
using TrainingTracker.Client.Server.DTOs.Sessions;
using System.Linq;
using System.Collections.Generic;

namespace TrainingTracker.Client.Server.Features.Sessions
{
    // 1. QUERY: Zwraca listę sesji dla danego użytkownika
    public record GetTrainingSessionsByUserQuery(int UserId) : IRequest<List<SessionDto>>;

    // 2. HANDLER
    public class GetTrainingSessionsByUserHandler : IRequestHandler<GetTrainingSessionsByUserQuery, List<SessionDto>>
    {
        private readonly ApplicationDbContext _context;

        public GetTrainingSessionsByUserHandler(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<List<SessionDto>> Handle(GetTrainingSessionsByUserQuery request, CancellationToken cancellationToken)
        {
            // Pobieanie sesji dla konkretnego UserId
            var sessions = await _context.TrainingSessions
                .Where(s => s.UserId == request.UserId)
                .OrderByDescending(s => s.StartedAt) // Najnowsze na górze
                .Select(s => new SessionDto
                {
                    Id = s.Id,
                    UserId = s.UserId,
                    StartedAt = s.StartedAt,
                    CompletedAt = s.CompletedAt,
                    Notes = s.Notes
                })
                .ToListAsync(cancellationToken);

            return sessions;
        }
    }
}