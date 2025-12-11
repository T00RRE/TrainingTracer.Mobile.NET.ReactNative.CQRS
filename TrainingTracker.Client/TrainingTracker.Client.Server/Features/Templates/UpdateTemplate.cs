using MediatR;
using FluentValidation;
using Microsoft.EntityFrameworkCore;
using TrainingTracker.Client.Server.Data;
using TrainingTracker.Client.Server.DTOs.Templates;

namespace TrainingTracker.Client.Server.Features.Templates
{
    // 1. COMMAND (Żądanie): Obejmuje ID zasobu i dane do aktualizacji. Zwraca 'Unit'.
    public record UpdateTemplateCommand(int Id, UpdateTemplateDto Data) : IRequest<Unit>;


    // 2. WALIDATOR: Sprawdza dane i upewnia się, że ID szablonu istnieje
    public class UpdateTemplateValidator : AbstractValidator<UpdateTemplateCommand>
    {
        private readonly ApplicationDbContext _context;

        public UpdateTemplateValidator(ApplicationDbContext context)
        {
            _context = context;

            RuleFor(x => x.Id)
                .GreaterThan(0).WithMessage("ID szablonu jest wymagane do aktualizacji.");

            RuleFor(x => x.Data.Name)
                .NotEmpty().WithMessage("Nazwa szablonu jest wymagana.")
                .MaximumLength(100).WithMessage("Nazwa jest za długa.");

            RuleFor(x => x.Id)
                .MustAsync(TemplateMustExist).WithMessage("Szablon o podanym ID nie został znaleziony.");
        }

        private async Task<bool> TemplateMustExist(int id, CancellationToken token)
        {
            return await _context.WorkoutTemplates.AnyAsync(t => t.Id == id, token);
        }
    }


    // 3. HANDLER (Obsługa Logiki)
    public class UpdateTemplateHandler : IRequestHandler<UpdateTemplateCommand, Unit>
    {
        private readonly ApplicationDbContext _context;

        public UpdateTemplateHandler(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<Unit> Handle(UpdateTemplateCommand request, CancellationToken cancellationToken)
        {
            // Znajdujemy szablon w bazie (Walidator zagwarantował, że istnieje)
            var templateToUpdate = await _context.WorkoutTemplates
                .FirstAsync(t => t.Id == request.Id, cancellationToken);

            // Aktualizacja właściwości
            templateToUpdate.Name = request.Data.Name;

            // Zapis zmian w bazie danych
            await _context.SaveChangesAsync(cancellationToken);

            // Zwracamy puste Unit (typ void dla MediatR)
            return Unit.Value;
        }
    }
}