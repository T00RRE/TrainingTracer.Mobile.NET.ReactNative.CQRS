using MediatR;
using Microsoft.AspNetCore.Mvc;
using TrainingTracker.Client.Server.DTOs.Categories;
using TrainingTracker.Client.Server.Features.Categories;

namespace TrainingTracker.Client.Server.Controllers
{
    [ApiController]
    [Route("api/[controller]")] // Endpoint: /api/ExerciseCategories
    public class ExerciseCategoriesController : ControllerBase
    {
        private readonly IMediator _mediator;

        public ExerciseCategoriesController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpPost] // POST /api/ExerciseCategories
        [ProducesResponseType(typeof(int), StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> CreateCategory([FromBody] CreateCategoryDto dto)
        {
            var command = new CreateExerciseCategoryCommand(dto);
            var newId = await _mediator.Send(command);

            return Created(string.Empty, newId);
        }

        // R - READ ALL
        [HttpGet] // GET /api/ExerciseCategories
        [ProducesResponseType(typeof(List<CategoryDto>), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetAllCategories()
        {
            var query = new GetAllCategoriesQuery();
            var result = await _mediator.Send(query);

            return Ok(result);
        }
        // U - UPDATE
        [HttpPut("{id}")] // PUT /api/ExerciseCategories/{id}
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> UpdateCategory(int id, [FromBody] CreateCategoryDto dto)
        {
            var command = new UpdateExerciseCategoryCommand(id, dto);
            await _mediator.Send(command);

            return NoContent();
        }

        // D - DELETE
        [HttpDelete("{id}")] // DELETE /api/ExerciseCategories/{id}
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> DeleteCategory(int id)
        {
            var command = new DeleteExerciseCategoryCommand(id);
            await _mediator.Send(command);

            return NoContent();
        }
    }
}