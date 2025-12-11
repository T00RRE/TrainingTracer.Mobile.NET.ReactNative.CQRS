using MediatR;
using Microsoft.AspNetCore.Mvc;
using TrainingTracker.Client.Server.DTOs.UserSettings;
using TrainingTracker.Client.Server.Features.UserSettings;

namespace TrainingTracker.Client.Server.Controllers
{
    [ApiController]
    [Route("api/[controller]")] // Endpoint: /api/UserSettings
    public class UserSettingsController : ControllerBase
    {
        private readonly IMediator _mediator;

        public UserSettingsController(IMediator mediator)
        {
            _mediator = mediator;
        }
        // R - READ (Pobierz ustawienia dla UserId)
        [HttpGet("{userId}")] // GET /api/UserSettings/{userId}
        [ProducesResponseType(typeof(UserSettingsDto), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetSettings(int userId)
        {
            var query = new GetUserSettingsQuery(userId);
            var result = await _mediator.Send(query);

            if (result == null)
            {
                return NotFound("Ustawienia dla tego użytkownika nie zostały znalezione.");
            }

            return Ok(result);
        }

        // C/U - CREATE/UPDATE (Upsert Settings)
        [HttpPut("{userId}")] // PUT /api/UserSettings/{userId}
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> CreateOrUpdateSettings(int userId, [FromBody] UpdateSettingsDto dto)
        {
            var command = new CreateOrUpdateUserSettingsCommand(userId, dto);
            await _mediator.Send(command);

            return NoContent(); // 204 No Content dla udanego Upsertu
        }

        // D - DELETE
        [HttpDelete("{userId}")] // DELETE /api/UserSettings/{userId}
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> DeleteSettings(int userId)
        {
            var command = new DeleteUserSettingsCommand(userId);
            await _mediator.Send(command);

            return NoContent();
        }
    }

}