using MediatR;
using FluentValidation;
using Microsoft.EntityFrameworkCore;
using TrainingTracker.Client.Server.Data;
using TrainingTracker.Client.Server.Models;
using TrainingTracker.Client.Server.DTOs.Templates;

namespace TrainingTracker.Client.Server.Features.Templates
{
    // 1. COMMAND (Żądanie): Obejmuje dane do utworzenia szablonu. Zwraca Id.
    public record CreateTemplateCommand(CreateTemplateDto Data) : IRequest<int>;


    // 2. WALIDATOR: Sprawdza nazwę i istnienie UserId w bazie
    public class CreateTemplateValidator : AbstractValidator<CreateTemplateCommand>
    {
        private readonly ApplicationDbContext _context;

        public CreateTemplateValidator(ApplicationDbContext context)
        {
            _context = context;

            RuleFor(x => x.Data.Name)
                .NotEmpty().WithMessage("Nazwa szablonu jest wymagana.")
                .MaximumLength(100).WithMessage("Nazwa jest za długa.");

            RuleFor(x => x.Data.UserId)
                .GreaterThan(0).WithMessage("ID użytkownika jest wymagane.")
                // Walidacja klucza obcego: Czy użytkownik o danym ID istnieje w bazie
                .MustAsync(UserMustExist).WithMessage("Użytkownik o podanym ID nie istnieje.");
        }

        // Metoda sprawdzająca istnienie użytkownika
        private async Task<bool> UserMustExist(CreateTemplateCommand command, int userId, CancellationToken token)
        {
            if (userId <= 0) return false;
            return await _context.Users.AnyAsync(u => u.Id == userId, token);
        }
    }


    // 3. HANDLER (Obsługa Logiki)
    public class CreateTemplateHandler : IRequestHandler<CreateTemplateCommand, int>
    {
        private readonly ApplicationDbContext _context;

        public CreateTemplateHandler(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<int> Handle(CreateTemplateCommand request, CancellationToken cancellationToken)
        {
            var newTemplate = new WorkoutTemplate
            {
                Name = request.Data.Name,
                UserId = request.Data.UserId,
                CreatedAt = DateTime.UtcNow // Domyślnie ustawiana w modelu, ale warto jawnie pokazać intencję
            };

            _context.WorkoutTemplates.Add(newTemplate);
            await _context.SaveChangesAsync(cancellationToken);

            return newTemplate.Id;
        }
    }
}