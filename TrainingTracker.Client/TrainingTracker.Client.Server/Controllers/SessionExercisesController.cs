using MediatR;
using Microsoft.AspNetCore.Mvc;
using TrainingTracker.Client.Server.DTOs.SessionExercises;
using TrainingTracker.Client.Server.Features.SessionExercises;

namespace TrainingTracker.Client.Server.Controllers
{
    [ApiController]
    [Route("api/[controller]")] // Endpoint: /api/SessionExercises
    public class SessionExercisesController : ControllerBase
    {
        private readonly IMediator _mediator;

        public SessionExercisesController(IMediator mediator)
        {
            _mediator = mediator;
        }

        // C - CREATE (Dodaj ćwiczenie do sesji)
        [HttpPost] // POST /api/SessionExercises
        [ProducesResponseType(typeof(int), StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> AddExerciseToSession([FromBody] AddSessionExerciseDto dto)
        {
            var command = new AddSessionExerciseCommand(dto);
            var newId = await _mediator.Send(command);

            return Created(string.Empty, newId);
        }

        // R - READ (Pobierz listę ćwiczeń w sesji)
        [HttpGet("{sessionId}")] // GET /api/SessionExercises/{sessionId}
        [ProducesResponseType(typeof(List<SessionExerciseDto>), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetExercisesInSession(int sessionId)
        {
            var query = new GetSessionExercisesQuery(sessionId);
            var result = await _mediator.Send(query);

            return Ok(result);
        }

        // D - DELETE (Usuń ćwiczenie z sesji)
        [HttpDelete("{id}")] // DELETE /api/SessionExercises/{id}
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> DeleteSessionExercise(int id)
        {
            var command = new DeleteSessionExerciseCommand(id);
            await _mediator.Send(command);

            return NoContent();
        }
    }
}