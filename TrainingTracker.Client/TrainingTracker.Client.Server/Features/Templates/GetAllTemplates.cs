using MediatR;
using Microsoft.EntityFrameworkCore;
using TrainingTracker.Client.Server.Data;
using TrainingTracker.Client.Server.DTOs.Templates;
using System.Linq;

namespace TrainingTracker.Client.Server.Features.Templates
{
    public record GetAllTemplatesQuery : IRequest<List<TemplateDto>>;

    public class GetAllTemplatesHandler : IRequestHandler<GetAllTemplatesQuery, List<TemplateDto>>
    {
        private readonly ApplicationDbContext _context;

        public GetAllTemplatesHandler(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<List<TemplateDto>> Handle(GetAllTemplatesQuery request, CancellationToken cancellationToken)
        {
            var templates = await _context.WorkoutTemplates
                .Select(t => new TemplateDto
                {
                    Id = t.Id,
                    Name = t.Name,
                    UserId = t.UserId,
                    CreatedAt = t.CreatedAt
                })
                .ToListAsync(cancellationToken);

            return templates;
        }
    }
}