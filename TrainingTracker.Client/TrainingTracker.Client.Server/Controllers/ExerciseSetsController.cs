using MediatR;
using Microsoft.AspNetCore.Mvc;
using TrainingTracker.Client.Server.DTOs.ExerciseSets;
using TrainingTracker.Client.Server.Features.ExerciseSets;

namespace TrainingTracker.Client.Server.Controllers
{
    [ApiController]
    [Route("api/[controller]")] // Endpoint: /api/ExerciseSets
    public class ExerciseSetsController : ControllerBase
    {
        private readonly IMediator _mediator;

        public ExerciseSetsController(IMediator mediator)
        {
            _mediator = mediator;
        }

        // C - CREATE (Dodaj serię)
        [HttpPost] // POST /api/ExerciseSets
        [ProducesResponseType(typeof(int), StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> AddSet([FromBody] AddSetDto dto)
        {
            var command = new AddExerciseSetCommand(dto);
            var newId = await _mediator.Send(command);

            return Created(string.Empty, newId);
        }

        // R - READ (Pobierz serie dla ćwiczenia w sesji)
        [HttpGet("sessionExercise/{sessionExerciseId}")] // GET /api/ExerciseSets/sessionExercise/{sessionExerciseId}
        [ProducesResponseType(typeof(List<SetDto>), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetSetsForSessionExercise(int sessionExerciseId)
        {
            var query = new GetSetsBySessionExerciseQuery(sessionExerciseId);
            var result = await _mediator.Send(query);

            return Ok(result);
        }

        // U - UPDATE (Aktualizuj serię)
        [HttpPut("{id}")] // PUT /api/ExerciseSets/{id}
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> UpdateSet(int id, [FromBody] UpdateSetDto dto)
        {
            var command = new UpdateExerciseSetCommand(id, dto);
            await _mediator.Send(command);

            return NoContent();
        }

        // D - DELETE (Usuń serię)
        [HttpDelete("{id}")] // DELETE /api/ExerciseSets/{id}
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> DeleteSet(int id)
        {
            var command = new DeleteExerciseSetCommand(id);
            await _mediator.Send(command);

            return NoContent();
        }
    }
}