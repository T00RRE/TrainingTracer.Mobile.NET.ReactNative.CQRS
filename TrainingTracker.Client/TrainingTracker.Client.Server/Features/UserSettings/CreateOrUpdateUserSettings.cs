using MediatR;
using FluentValidation;
using Microsoft.EntityFrameworkCore;
using TrainingTracker.Client.Server.Data;
using TrainingTracker.Client.Server.Models;
using TrainingTracker.Client.Server.DTOs.UserSettings;

namespace TrainingTracker.Client.Server.Features.UserSettings
{
    // 1. COMMAND: Używa UserId w ścieżce, dane w body
    public record CreateOrUpdateUserSettingsCommand(int UserId, UpdateSettingsDto Data) : IRequest<Unit>;

    // 2. WALIDATOR (Sprawdza UserId i poprawność danych)
    public class CreateOrUpdateUserSettingsValidator : AbstractValidator<CreateOrUpdateUserSettingsCommand>
    {
        private readonly ApplicationDbContext _context;

        public CreateOrUpdateUserSettingsValidator(ApplicationDbContext context)
        {
            _context = context;

            RuleFor(x => x.UserId)
                .GreaterThan(0).WithMessage("ID użytkownika jest wymagane.")
                .MustAsync(UserMustExist).WithMessage("Użytkownik o podanym ID nie istnieje.");

            RuleFor(x => x.Data.WeightUnit)
                .NotEmpty().WithMessage("Jednostka wagi jest wymagana.");

            RuleFor(x => x.Data.Theme)
                .NotEmpty().WithMessage("Motyw jest wymagany.");
        }

        private async Task<bool> UserMustExist(CreateOrUpdateUserSettingsCommand command, int userId, CancellationToken token)
        {
            return await _context.Users.AnyAsync(u => u.Id == userId, token);
        }
    }


    // 3. HANDLER (Logika Upsert: Znajdź lub utwórz)
    public class CreateOrUpdateUserSettingsHandler : IRequestHandler<CreateOrUpdateUserSettingsCommand, Unit>
    {
        private readonly ApplicationDbContext _context;

        public CreateOrUpdateUserSettingsHandler(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<Unit> Handle(CreateOrUpdateUserSettingsCommand request, CancellationToken cancellationToken)
        {
            var settings = await _context.UserSettings
                .FirstOrDefaultAsync(s => s.UserId == request.UserId, cancellationToken);

            if (settings == null)
            {
                // CREATE: Jeśli ustawienia nie istnieją, tworzymy nowe
                settings = new Models.UserSettings
                {
                    UserId = request.UserId,
                    WeightUnit = request.Data.WeightUnit,
                    Theme = request.Data.Theme
                };
                _context.UserSettings.Add(settings);
            }
            else
            {
                // UPDATE: Jeśli ustawienia istnieją, aktualizujemy je
                settings.WeightUnit = request.Data.WeightUnit;
                settings.Theme = request.Data.Theme;
            }

            await _context.SaveChangesAsync(cancellationToken);
            return Unit.Value;
        }
    }
}