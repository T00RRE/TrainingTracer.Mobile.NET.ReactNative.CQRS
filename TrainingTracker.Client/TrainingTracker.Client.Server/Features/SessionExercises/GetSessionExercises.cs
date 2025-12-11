using MediatR;
using Microsoft.EntityFrameworkCore;
using TrainingTracker.Client.Server.Data;
using TrainingTracker.Client.Server.DTOs.SessionExercises;

namespace TrainingTracker.Client.Server.Features.SessionExercises
{
    // 1. QUERY: Pobiera wszystkie ćwiczenia dla danej sesji
    public record GetSessionExercisesQuery(int SessionId) : IRequest<List<SessionExerciseDto>>;

    // 2. HANDLER
    public class GetSessionExercisesHandler : IRequestHandler<GetSessionExercisesQuery, List<SessionExerciseDto>>
    {
        private readonly ApplicationDbContext _context;

        public GetSessionExercisesHandler(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<List<SessionExerciseDto>> Handle(GetSessionExercisesQuery request, CancellationToken cancellationToken)
        {
            var exercisesInSession = await _context.SessionExercises
                .Where(se => se.SessionId == request.SessionId)
                .Include(se => se.Exercise) // Pobieramy nazwę ćwiczenia
                .OrderBy(se => se.OrderPosition)
                .Select(se => new SessionExerciseDto
                {
                    Id = se.Id,
                    SessionId = se.SessionId,
                    ExerciseId = se.ExerciseId,
                    ExerciseName = se.Exercise.Name,
                    OrderPosition = se.OrderPosition
                })
                .ToListAsync(cancellationToken);

            return exercisesInSession;
        }
    }
}