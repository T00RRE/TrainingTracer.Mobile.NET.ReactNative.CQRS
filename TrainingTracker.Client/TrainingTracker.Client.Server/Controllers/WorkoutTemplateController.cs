using MediatR;
using Microsoft.AspNetCore.Mvc;
using TrainingTracker.Client.Server.DTOs.Templates;
using TrainingTracker.Client.Server.Features.Templates;

namespace TrainingTracker.Client.Server.Controllers
{
    [ApiController]
    [Route("api/[controller]")] // Endpoint: /api/WorkoutTemplates
    public class WorkoutTemplatesController : ControllerBase
    {
        private readonly IMediator _mediator;

        public WorkoutTemplatesController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpGet] // GET /api/WorkoutTemplates
        [ProducesResponseType(typeof(List<TemplateDto>), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetAllTemplates()
        {
            var result = await _mediator.Send(new GetAllTemplatesQuery());
            return Ok(result);
        }

        [HttpPost]
        [ProducesResponseType(typeof(int), StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> CreateTemplate([FromBody] CreateTemplateDto templateDto)
        {
            var command = new CreateTemplateCommand(templateDto);

            // MediatR automatycznie uruchomi Walidator
            var newId = await _mediator.Send(command);

            return CreatedAtAction(nameof(GetAllTemplates), new { id = newId }, newId);
        }
        [HttpPut("{id}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> UpdateTemplate(int id, [FromBody] UpdateTemplateDto templateDto)
        {
            // 1. Tworzymy Command z ID i danymi
            var command = new UpdateTemplateCommand(id, templateDto);

            // 2. Wysyłamy Command do MediatR
            await _mediator.Send(command);

            // 3. Zwracamy status 204 No Content
            return NoContent();
        }
        [HttpDelete("{id}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> DeleteTemplate(int id)
        {
            // 1. Tworzymy Command tylko z ID
            var command = new DeleteTemplateCommand(id);

            // 2. Wysyłamy Command do MediatR
            await _mediator.Send(command);

            // 3. Zwracamy status 204 No Content
            return NoContent();
        }
    }
}