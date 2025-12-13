using MediatR;
using Microsoft.AspNetCore.Mvc;
using TrainingTracker.Client.Server.DTOs.Users;
using TrainingTracker.Client.Server.Features.Users;

namespace TrainingTracker.Client.Server.Controllers
{
    [ApiController]
    [Route("api/[controller]")] // Endpoint: /api/Users
    public class UsersController : ControllerBase
    {
        private readonly IMediator _mediator;

        public UsersController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpPost] // POST /api/Users
        [ProducesResponseType(typeof(int), StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> CreateUser([FromBody] RegisterUserDto userDto)
        {
            var command = new RegisterUserCommand(userDto);
            var newId = await _mediator.Send(command);

            return Created(string.Empty, newId);
        }

        [HttpGet("{id}")] // GET /api/Users/{id}
        [ProducesResponseType(typeof(UserDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetUser(int id)
        {
            var query = new GetUserByIdQuery(id);
            var result = await _mediator.Send(query);

            if (result == null)
            {
                return NotFound();
            }

            return Ok(result);
        }
        [HttpPut("{id}")] // PUT /api/Users/{id}
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> UpdateUser(int id, [FromBody] UpdateUserDto userDto)
        {
            var command = new UpdateUserCommand(id, userDto);
            await _mediator.Send(command);

            return NoContent();
        }
        [HttpDelete("{id}")] // DELETE /api/Users/{id}
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> DeleteUser(int id)
        {
            var command = new DeleteUserCommand(id);
            await _mediator.Send(command);

            return NoContent();
        }
    }
}