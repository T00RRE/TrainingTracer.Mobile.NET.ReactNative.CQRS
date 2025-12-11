using MediatR;
using Microsoft.AspNetCore.Mvc;
using TrainingTracker.Client.Server.DTOs.Exercises;
using TrainingTracker.Client.Server.Features.Exercises;

namespace TrainingTracker.Client.Server.Controllers
{
    [ApiController]
    [Route("api/[controller]")] // Endpoint: /api/Exercises
    public class ExercisesController : ControllerBase
    {
        private readonly IMediator _mediator;

        public ExercisesController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpGet] // Endpoint: GET /api/Exercises
        [ProducesResponseType(typeof(List<ExerciseDto>), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetAllExercises()
        {
            // Wysyłamy Query do MediatR
            var result = await _mediator.Send(new GetAllExercisesQuery());

            return Ok(result);
        }

        [HttpPost]
        [ProducesResponseType(typeof(int), StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> CreateExercise([FromBody] CreateExerciseDto exerciseDto)
        {
            var command = new CreateExerciseCommand(exerciseDto);

            var newId = await _mediator.Send(command);

            return CreatedAtAction(nameof(GetAllExercises), new { id = newId }, newId);
        }

        [HttpPut("{id}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)] // Zwraca 204 No Content dla udanej aktualizacji
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)] // Zwraca 404 jeśli walidacja nie znajdzie ID
        public async Task<IActionResult> UpdateExercise(int id, [FromBody] UpdateExerciseDto exerciseDto)
        {
            // 1. Tworzymy Command z ID i danymi
            var command = new UpdateExerciseCommand(id, exerciseDto);

            // 2. Wysyłamy Command do MediatR
            await _mediator.Send(command);

            // 3. Zwracamy status 204 No Content
            return NoContent();
        }
        [HttpDelete("{id}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)] // Zwraca 204 No Content dla udanego usunięcia
        [ProducesResponseType(StatusCodes.Status404NotFound)] // Zwraca 404, jeśli walidacja nie znajdzie ID
        public async Task<IActionResult> DeleteExercise(int id)
        {
            // 1. Tworzymy Command tylko z ID
            var command = new DeleteExerciseCommand(id);

            // 2. Wysyłamy Command do MediatR
            await _mediator.Send(command);

            // 3. Zwracamy status 204 No Content
            return NoContent();
        }
    }
}