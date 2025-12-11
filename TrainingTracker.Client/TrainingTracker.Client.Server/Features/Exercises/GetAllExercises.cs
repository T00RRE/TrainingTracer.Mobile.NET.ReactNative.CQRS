using MediatR;
using Microsoft.EntityFrameworkCore;
using TrainingTracker.Client.Server.Data;
using TrainingTracker.Client.Server.DTOs.Exercises;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using System.Linq; 

namespace TrainingTracker.Client.Server.Features.Exercises
{
    // 1. QUERY (Zapytanie)
    public record GetAllExercisesQuery : IRequest<List<ExerciseDto>>;

    // 2. HANDLER (Obsługa Logiki)
    public class GetAllExercisesHandler : IRequestHandler<GetAllExercisesQuery, List<ExerciseDto>>
    {
        private readonly ApplicationDbContext _context;

        public GetAllExercisesHandler(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<List<ExerciseDto>> Handle(GetAllExercisesQuery request, CancellationToken cancellationToken)
        {
            // Łączy z tabelą Category i rzutuje na DTO
            var exercises = await _context.Exercises
                .Include(e => e.Category)
                .Select(e => new ExerciseDto
                {
                    Id = e.Id,
                    Name = e.Name,
                    Description = e.Description,
                    IsGlobal = e.IsGlobal,
                    CategoryName = e.Category.Name
                })
                .ToListAsync(cancellationToken);

            return exercises;
        }
    }
}