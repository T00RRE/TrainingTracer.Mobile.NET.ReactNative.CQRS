using Microsoft.EntityFrameworkCore;
using TrainingTracker.Client.Server.Models; // Będziemy tu importować Twoje modele

namespace TrainingTracker.Client.Server.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }
        public DbSet<User> Users { get; set; }
        public DbSet<WorkoutTemplate> WorkoutTemplates { get; set; }
        public DbSet<ExerciseCategory> ExerciseCategories { get; set; }
        public DbSet<Exercise> Exercises { get; set; }
        public DbSet<TemplateExercise> TemplateExercises { get; set; }
        public DbSet<TrainingSession> TrainingSessions { get; set; }
        public DbSet<SessionExercise> SessionExercises { get; set; }
        public DbSet<ExerciseSet> ExerciseSets { get; set; }
        public DbSet<UserSettings> UserSettings { get; set; }
        public DbSet<WorkoutType> WorkoutTypes { get; set; }


    }
}