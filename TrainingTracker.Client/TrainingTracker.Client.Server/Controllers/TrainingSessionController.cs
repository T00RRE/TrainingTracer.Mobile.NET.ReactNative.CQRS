using MediatR;
using Microsoft.AspNetCore.Mvc;
using TrainingTracker.Client.Server.DTOs.Sessions;
using TrainingTracker.Client.Server.Features.Sessions; // Ten using dodamy w Kroku 3

namespace TrainingTracker.Client.Server.Controllers
{
    [ApiController]
    [Route("api/[controller]")] // Endpoint: /api/TrainingSessions
    public class TrainingSessionsController : ControllerBase
    {
        private readonly IMediator _mediator;

        public TrainingSessionsController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpPost] // POST /api/TrainingSessions
        [ProducesResponseType(typeof(int), StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> StartSession([FromBody] StartSessionDto sessionDto)
        {
            var command = new StartTrainingSessionCommand(sessionDto);
            var newId = await _mediator.Send(command);

            return Created(string.Empty, newId);
        }
        [HttpGet("user/{userId}")]
        [ProducesResponseType(typeof(List<SessionDto>), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetUserSessions(int userId)
        {
            var query = new GetTrainingSessionsByUserQuery(userId);
            var result = await _mediator.Send(query);

            return Ok(result);
        }
        [HttpPut("{id}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> UpdateSession(int id, [FromBody] UpdateSessionDto sessionDto)
        {
            var command = new UpdateTrainingSessionCommand(id, sessionDto);
            await _mediator.Send(command);

            return NoContent();
        }
        [HttpDelete("{id}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> DeleteSession(int id)
        {
            var command = new DeleteTrainingSessionCommand(id);
            await _mediator.Send(command);

            return NoContent();
        }
    }
}