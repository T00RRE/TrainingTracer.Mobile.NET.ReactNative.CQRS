using MediatR;
using Microsoft.EntityFrameworkCore;
using System;
using TrainingTracker.Client.Server.Data; // Upewnij się, że tu jest Twój DbContext
using TrainingTracker.Client.Server.DTOs.Sessions;

namespace TrainingTracker.Client.Server.Features.Sessions
{
    public class GetWorkoutSessionDetailsHandler : IRequestHandler<GetWorkoutSessionDetailsQuery, WorkoutSessionDetailsDto>
    {
        private readonly ApplicationDbContext _context; // Podstaw nazwę swojego DbContextu

        public GetWorkoutSessionDetailsHandler(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<WorkoutSessionDetailsDto> Handle(GetWorkoutSessionDetailsQuery request, CancellationToken cancellationToken)
        {
            // 1. Pobieramy nagłówek sesji
            var session = await _context.TrainingSessions
                .Where(s => s.Id == request.SessionId)
                .Select(s => new { s.StartedAt })
                .FirstOrDefaultAsync(cancellationToken);

            if (session == null) return null;

            // 2. Pobieramy nazwę szablonu
            var templateName = await _context.WorkoutTemplates
                .Where(wt => wt.Id == request.TemplateId)
                .Select(wt => wt.Name)
                .FirstOrDefaultAsync(cancellationToken);

            // 3. Pobieramy listę ćwiczeń z podzapytaniami (logika Twojego SQL)
            var exercises = await _context.SessionExercises
                .Where(se => se.SessionId == request.SessionId)
                .OrderBy(se => se.OrderPosition)
                .Select(se => new SessionExerciseRowDto
                {
                    SessionExerciseId = se.Id,
                    Cwiczenie = se.Exercise.Name,


                    // Odpowiednik COALESCE(te.DefaultSets, 0)
                    PlanowaneSerie = _context.TemplateExercises
                        .Where(te => te.TemplateId == request.TemplateId && te.ExerciseId == se.ExerciseId)
                        .Select(te => (int?)te.DefaultSets)
                        .FirstOrDefault() ?? 0,

                    // Liczba serii w tej konkretnej sesji
                    WykonaneSerie = _context.ExerciseSets
                    .Where(es => es.SessionExerciseId == se.Id)
                    .Sum(es => (int?)es.Reps) ?? 0,

                    // Ostatni ciężar z historii tego ćwiczenia
                    OstatniCiezar = _context.ExerciseSets
                        .Where(es_old => es_old.SessionExercise.ExerciseId == se.ExerciseId)
                        .OrderByDescending(es_old => es_old.CompletedAt)
                        .Select(es_old => (double?)es_old.Weight)
                        .FirstOrDefault()
                })
                .ToListAsync(cancellationToken);

            return new WorkoutSessionDetailsDto
            {
                NazwaSzablonu = templateName ?? "Trening",
                DataTreningu = session.StartedAt,
                Cwiczenia = exercises
            };
        }
    }
}