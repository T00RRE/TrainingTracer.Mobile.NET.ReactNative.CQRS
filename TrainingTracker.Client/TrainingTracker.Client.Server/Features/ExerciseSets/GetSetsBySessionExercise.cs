using MediatR;
using Microsoft.EntityFrameworkCore;
using TrainingTracker.Client.Server.Data;
using TrainingTracker.Client.Server.DTOs.ExerciseSets;

namespace TrainingTracker.Client.Server.Features.ExerciseSets
{
    // 1. QUERY: Pobiera wszystkie serie dla danego ćwiczenia w sesji
    public record GetSetsBySessionExerciseQuery(int SessionExerciseId) : IRequest<List<SetDto>>;

    // 2. HANDLER
    public class GetSetsBySessionExerciseHandler : IRequestHandler<GetSetsBySessionExerciseQuery, List<SetDto>>
    {
        private readonly ApplicationDbContext _context;

        public GetSetsBySessionExerciseHandler(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<List<SetDto>> Handle(GetSetsBySessionExerciseQuery request, CancellationToken cancellationToken)
        {
            var sets = await _context.ExerciseSets
                .Where(s => s.SessionExerciseId == request.SessionExerciseId)
                .OrderBy(s => s.SetNumber)
                .Select(s => new SetDto
                {
                    Id = s.Id,
                    SessionExerciseId = s.SessionExerciseId,
                    SetNumber = s.SetNumber,
                    Weight = s.Weight,
                    Reps = s.Reps,
                    CompletedAt = s.CompletedAt
                })
                .ToListAsync(cancellationToken);

            return sets;
        }
    }
}