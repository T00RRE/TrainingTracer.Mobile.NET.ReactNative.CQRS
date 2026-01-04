using MediatR;
using Microsoft.EntityFrameworkCore;
using TrainingTracker.Client.Server.Data;
using TrainingTracker.Client.Server.DTOs.ExerciseSets;
// Dodaj odpowiednie usingi dla Twojego Contextu i DTO

public class GetTrainingSessionDetailsQuery : IRequest<TrainingSessionDetailsDto>
{
    public int SessionId { get; set; }
}

public class GetTrainingSessionDetailsHandler : IRequestHandler<GetTrainingSessionDetailsQuery, TrainingSessionDetailsDto>
{
    private readonly ApplicationDbContext _context;

    public GetTrainingSessionDetailsHandler(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<TrainingSessionDetailsDto> Handle(GetTrainingSessionDetailsQuery request, CancellationToken cancellationToken)
    {
        // Pobieramy sesję z całym "drzewem" relacji
        var session = await _context.TrainingSessions
            .Include(s => s.Template)
            .Include(s => s.SessionExercises)
                .ThenInclude(se => se.Exercise)
            .Include(s => s.SessionExercises)
                .ThenInclude(se => se.ExerciseSets)
            .FirstOrDefaultAsync(s => s.Id == request.SessionId, cancellationToken);

        if (session == null) return null!;

        return new TrainingSessionDetailsDto
        {
            Id = session.Id,
            TemplateName = session.Template?.Name ?? "Trening własny",
            Date = session.StartedAt,
            // Obliczamy czas trwania jeśli trening jest zakończony
            Duration = session.CompletedAt.HasValue
                ? (session.CompletedAt.Value - session.StartedAt).ToString(@"hh\:mm")
                : "W trakcie",
            Notes = session.Notes,
            Exercises = session.SessionExercises.Select(se => new SessionExerciseDetailsDto
            {
                ExerciseName = se.Exercise.Name,
                Sets = se.ExerciseSets.OrderBy(set => set.SetNumber).Select(set => new SetDto
                {
                    Reps = set.Reps,
                    Weight = set.Weight
                }).ToList()
            }).ToList()
        };
    }
}
