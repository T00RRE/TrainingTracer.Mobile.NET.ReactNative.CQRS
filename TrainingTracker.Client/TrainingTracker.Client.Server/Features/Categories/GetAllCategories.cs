using MediatR;
using Microsoft.EntityFrameworkCore;
using TrainingTracker.Client.Server.Data;
using TrainingTracker.Client.Server.DTOs.Categories;
using System.Linq;

namespace TrainingTracker.Client.Server.Features.Categories
{
    public record GetAllCategoriesQuery : IRequest<List<CategoryDto>>;

    public class GetAllCategoriesHandler : IRequestHandler<GetAllCategoriesQuery, List<CategoryDto>>
    {
        private readonly ApplicationDbContext _context;

        public GetAllCategoriesHandler(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<List<CategoryDto>> Handle(GetAllCategoriesQuery request, CancellationToken cancellationToken)
        {
            var categories = await _context.ExerciseCategories
                .Select(c => new CategoryDto
                {
                    Id = c.Id,
                    Name = c.Name
                })
                .ToListAsync(cancellationToken);

            return categories;
        }
    }
}