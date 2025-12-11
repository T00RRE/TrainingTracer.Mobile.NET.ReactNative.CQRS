using MediatR;
using Microsoft.AspNetCore.Mvc;
using TrainingTracker.Client.Server.DTOs.WorkoutTypes;
using TrainingTracker.Client.Server.Features.WorkoutTypes;

namespace TrainingTracker.Client.Server.Controllers
{
    [ApiController]
    [Route("api/[controller]")] // Endpoint: /api/WorkoutTypes
    public class WorkoutTypesController : ControllerBase
    {
        private readonly IMediator _mediator;

        public WorkoutTypesController(IMediator mediator)
        {
            _mediator = mediator;
        }

        // C - CREATE
        [HttpPost] // POST /api/WorkoutTypes
        [ProducesResponseType(typeof(int), StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> CreateWorkoutType([FromBody] CreateWorkoutTypeDto dto)
        {
            var command = new CreateWorkoutTypeCommand(dto);
            var newId = await _mediator.Send(command);

            return Created(string.Empty, newId);
        }

        // R - READ ALL
        [HttpGet] // GET /api/WorkoutTypes
        [ProducesResponseType(typeof(List<WorkoutTypeDto>), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetAllWorkoutTypes()
        {
            var query = new GetAllWorkoutTypesQuery();
            var result = await _mediator.Send(query);

            return Ok(result);
        }
        // U - UPDATE
        [HttpPut("{id}")] // PUT /api/WorkoutTypes/{id}
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> UpdateWorkoutType(int id, [FromBody] CreateWorkoutTypeDto dto)
        {
            var command = new UpdateWorkoutTypeCommand(id, dto);
            await _mediator.Send(command);

            return NoContent();
        }

        // D - DELETE
        [HttpDelete("{id}")] // DELETE /api/WorkoutTypes/{id}
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> DeleteWorkoutType(int id)
        {
            var command = new DeleteWorkoutTypeCommand(id);
            await _mediator.Send(command);

            return NoContent();
        }
    }
}