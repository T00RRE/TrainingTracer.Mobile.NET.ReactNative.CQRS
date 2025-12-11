using MediatR;
using FluentValidation;
using Microsoft.EntityFrameworkCore;
using TrainingTracker.Client.Server.Data;

namespace TrainingTracker.Client.Server.Features.Templates
{
    // 1. COMMAND (Żądanie): Obejmuje ID zasobu do usunięcia. Zwraca 'Unit'.
    public record DeleteTemplateCommand(int Id) : IRequest<Unit>;


    // 2. WALIDATOR: Zapewnia, że szablon istnieje przed próbą usunięcia
    public class DeleteTemplateValidator : AbstractValidator<DeleteTemplateCommand>
    {
        private readonly ApplicationDbContext _context;

        public DeleteTemplateValidator(ApplicationDbContext context)
        {
            _context = context;

            RuleFor(x => x.Id)
                .GreaterThan(0).WithMessage("ID szablonu do usunięcia jest wymagane.")
                .MustAsync(TemplateMustExist).WithMessage("Szablon o podanym ID nie został znaleziony.");
        }

        // Metoda sprawdzająca, czy szablon istnieje
        private async Task<bool> TemplateMustExist(int id, CancellationToken token)
        {
            return await _context.WorkoutTemplates.AnyAsync(t => t.Id == id, token);
        }
    }


    // 3. HANDLER (Obsługa Logiki)
    public class DeleteTemplateHandler : IRequestHandler<DeleteTemplateCommand, Unit>
    {
        private readonly ApplicationDbContext _context;

        public DeleteTemplateHandler(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<Unit> Handle(DeleteTemplateCommand request, CancellationToken cancellationToken)
        {
            // Znajdujemy encję do usunięcia (Walidator gwarantuje, że istnieje)
            var templateToDelete = await _context.WorkoutTemplates
                .FirstAsync(t => t.Id == request.Id, cancellationToken);

            // Usuwanie encji
            _context.WorkoutTemplates.Remove(templateToDelete);

            // Zapis zmian w bazie danych
            await _context.SaveChangesAsync(cancellationToken);

            return Unit.Value;
        }
    }
}