
namespace TrainingTracker.Client.Server.Models
{
    public class SessionExercise
    {
        public int Id { get; set; }
        public int SessionId { get; set; } // Foreign Key do TrainingSession
        public int ExerciseId { get; set; } // Foreign Key do Exercise
        public int OrderPosition { get; set; }

        // Relacje
        public TrainingSession Session { get; set; }
        public Exercise Exercise { get; set; }
        public ICollection<ExerciseSet> ExerciseSets { get; set; }
    }
}
