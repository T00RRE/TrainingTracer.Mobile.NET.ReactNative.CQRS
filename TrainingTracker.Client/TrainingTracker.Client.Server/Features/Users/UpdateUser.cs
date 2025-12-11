using MediatR;
using FluentValidation;
using Microsoft.EntityFrameworkCore;
using TrainingTracker.Client.Server.Data;
using TrainingTracker.Client.Server.DTOs.Users;

namespace TrainingTracker.Client.Server.Features.Users
{
    public record UpdateUserCommand(int Id, UpdateUserDto Data) : IRequest<Unit>;

    public class UpdateUserValidator : AbstractValidator<UpdateUserCommand>
    {
        private readonly ApplicationDbContext _context;

        public UpdateUserValidator(ApplicationDbContext context)
        {
            _context = context;

            RuleFor(x => x.Id)
                .GreaterThan(0).WithMessage("ID użytkownika jest wymagane do aktualizacji.")
                .MustAsync(UserMustExist).WithMessage("Użytkownik o podanym ID nie został znaleziony.");

            RuleFor(x => x.Data.Email)
                .EmailAddress().WithMessage("Wprowadź poprawny format email.")
                .MustAsync(IsEmailUniqueWhenUpdating).WithMessage("Ten email jest już używany przez innego użytkownika.");
        }

        private async Task<bool> UserMustExist(int id, CancellationToken token)
        {
            return await _context.Users.AnyAsync(u => u.Id == id, token);
        }

        private async Task<bool> IsEmailUniqueWhenUpdating(UpdateUserCommand command, string email, CancellationToken token)
        {
            if (string.IsNullOrEmpty(email)) return true;
            // Sprawdza, czy email jest unikalny DLA WSZYSTKICH innych użytkowników
            return !await _context.Users.AnyAsync(u => u.Email == email && u.Id != command.Id, token);
        }
    }

    public class UpdateUserHandler : IRequestHandler<UpdateUserCommand, Unit>
    {
        private readonly ApplicationDbContext _context;

        public UpdateUserHandler(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<Unit> Handle(UpdateUserCommand request, CancellationToken cancellationToken)
        {
            var userToUpdate = await _context.Users
                .FirstAsync(u => u.Id == request.Id, cancellationToken);

            userToUpdate.Username = request.Data.Username ?? userToUpdate.Username;
            userToUpdate.Email = request.Data.Email ?? userToUpdate.Email;

            if (!string.IsNullOrEmpty(request.Data.Password))
            {
                // Uproszczona zmiana hasła dla DEMO
                userToUpdate.PasswordHash = request.Data.Password;
            }

            await _context.SaveChangesAsync(cancellationToken);
            return Unit.Value;
        }
    }
}