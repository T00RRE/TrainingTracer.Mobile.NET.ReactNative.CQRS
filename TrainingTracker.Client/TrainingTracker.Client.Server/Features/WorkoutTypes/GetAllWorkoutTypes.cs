using MediatR;
using Microsoft.EntityFrameworkCore;
using TrainingTracker.Client.Server.Data;
using TrainingTracker.Client.Server.DTOs.WorkoutTypes;
using System.Linq;

namespace TrainingTracker.Client.Server.Features.WorkoutTypes
{
    public record GetAllWorkoutTypesQuery : IRequest<List<WorkoutTypeDto>>;

    public class GetAllWorkoutTypesHandler : IRequestHandler<GetAllWorkoutTypesQuery, List<WorkoutTypeDto>>
    {
        private readonly ApplicationDbContext _context;

        public GetAllWorkoutTypesHandler(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<List<WorkoutTypeDto>> Handle(GetAllWorkoutTypesQuery request, CancellationToken cancellationToken)
        {
            var types = await _context.WorkoutTypes
                .Select(t => new WorkoutTypeDto
                {
                    Id = t.Id,
                    Name = t.Name
                })
                .ToListAsync(cancellationToken);

            return types;
        }
    }
}