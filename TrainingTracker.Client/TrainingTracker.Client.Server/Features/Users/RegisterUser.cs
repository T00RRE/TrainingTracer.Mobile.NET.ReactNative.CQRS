using MediatR;
using FluentValidation;
using Microsoft.EntityFrameworkCore;
using TrainingTracker.Client.Server.Data;
using TrainingTracker.Client.Server.Models;
using TrainingTracker.Client.Server.DTOs.Users;

namespace TrainingTracker.Client.Server.Features.Users
{
    // 1. COMMAND (Żądanie): Zwraca ID nowo zarejestrowanego użytkownika
    public record RegisterUserCommand(RegisterUserDto Data) : IRequest<int>;


    // 2. WALIDATOR: Sprawdza unikalność emaila i siłę hasła
    public class RegisterUserValidator : AbstractValidator<RegisterUserCommand>
    {
        private readonly ApplicationDbContext _context;

        public RegisterUserValidator(ApplicationDbContext context)
        {
            _context = context;

            RuleFor(x => x.Data.Email)
                .NotEmpty().WithMessage("Email jest wymagany.")
                .EmailAddress().WithMessage("Wprowadź poprawny format email.")
                .MustAsync(IsEmailUnique).WithMessage("Ten email jest już zarejestrowany.");

            RuleFor(x => x.Data.Password)
                .NotEmpty().WithMessage("Hasło jest wymagane.")
                .MinimumLength(3).WithMessage("Hasło musi mieć co najmniej 3 znaki.");
        }

        private async Task<bool> IsEmailUnique(RegisterUserCommand command, string email, CancellationToken token)
        {
            return !await _context.Users.AnyAsync(u => u.Email == email, token);
        }
    }


    // 3. HANDLER (Obsługa Logiki - UPROSZCZONA BEZ HASZOWANIA)
    public class RegisterUserHandler : IRequestHandler<RegisterUserCommand, int>
    {
        private readonly ApplicationDbContext _context;

        public RegisterUserHandler(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<int> Handle(RegisterUserCommand request, CancellationToken cancellationToken)
        {
            var newUser = new User
            {
                Username = request.Data.Username,
                Email = request.Data.Email,
                // PROSTA WERSJA DLA CELÓW DEMONSTRACYJNYCH: 
                // Zapisujemy hasło bez haszowania w polu PasswordHash
                PasswordHash = request.Data.Password,
                CreatedAt = DateTime.UtcNow
            };

            _context.Users.Add(newUser);
            await _context.SaveChangesAsync(cancellationToken);

            return newUser.Id;
        }
    }
}