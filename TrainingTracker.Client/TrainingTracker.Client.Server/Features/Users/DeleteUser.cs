using MediatR;
using FluentValidation;
using Microsoft.EntityFrameworkCore;
using TrainingTracker.Client.Server.Data;

namespace TrainingTracker.Client.Server.Features.Users
{
    public record DeleteUserCommand(int Id) : IRequest<Unit>;

    public class DeleteUserValidator : AbstractValidator<DeleteUserCommand>
    {
        private readonly ApplicationDbContext _context;

        public DeleteUserValidator(ApplicationDbContext context)
        {
            _context = context;

            RuleFor(x => x.Id)
                .GreaterThan(0).WithMessage("ID użytkownika jest wymagane do usunięcia.")
                .MustAsync(UserMustExist).WithMessage("Użytkownik o podanym ID nie został znaleziony.");
        }

        private async Task<bool> UserMustExist(int id, CancellationToken token)
        {
            return await _context.Users.AnyAsync(u => u.Id == id, token);
        }
    }

    public class DeleteUserHandler : IRequestHandler<DeleteUserCommand, Unit>
    {
        private readonly ApplicationDbContext _context;

        public DeleteUserHandler(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<Unit> Handle(DeleteUserCommand request, CancellationToken cancellationToken)
        {
            var userToDelete = await _context.Users
                .FirstAsync(u => u.Id == request.Id, cancellationToken);

            _context.Users.Remove(userToDelete);
            // CASCADE DELETE: Usunięcie użytkownika usunie jego WorkoutTemplates i TrainingSessions
            await _context.SaveChangesAsync(cancellationToken);

            return Unit.Value;
        }
    }
}