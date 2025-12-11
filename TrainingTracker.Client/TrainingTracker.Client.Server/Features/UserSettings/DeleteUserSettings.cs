using MediatR;
using FluentValidation;
using Microsoft.EntityFrameworkCore;
using TrainingTracker.Client.Server.Data;

namespace TrainingTracker.Client.Server.Features.UserSettings
{
    // 1. COMMAND
    public record DeleteUserSettingsCommand(int UserId) : IRequest<Unit>;

    // 2. WALIDATOR
    public class DeleteUserSettingsValidator : AbstractValidator<DeleteUserSettingsCommand>
    {
        private readonly ApplicationDbContext _context;

        public DeleteUserSettingsValidator(ApplicationDbContext context)
        {
            _context = context;

            RuleFor(x => x.UserId)
                .GreaterThan(0).WithMessage("ID użytkownika jest wymagane do usunięcia ustawień.")
                .MustAsync(UserMustExistAndHaveSettings).WithMessage("Użytkownik nie posiada ustawień do usunięcia.");
        }

        private async Task<bool> UserMustExistAndHaveSettings(int userId, CancellationToken token)
        {
            // Sprawdzamy czy istnieje ustawienie dla tego użytkownika
            return await _context.UserSettings.AnyAsync(s => s.UserId == userId, token);
        }
    }

    // 3. HANDLER
    public class DeleteUserSettingsHandler : IRequestHandler<DeleteUserSettingsCommand, Unit>
    {
        private readonly ApplicationDbContext _context;

        public DeleteUserSettingsHandler(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<Unit> Handle(DeleteUserSettingsCommand request, CancellationToken cancellationToken)
        {
            var settingsToDelete = await _context.UserSettings
                .FirstAsync(s => s.UserId == request.UserId, cancellationToken);

            _context.UserSettings.Remove(settingsToDelete);
            await _context.SaveChangesAsync(cancellationToken);

            return Unit.Value;
        }
    }
}