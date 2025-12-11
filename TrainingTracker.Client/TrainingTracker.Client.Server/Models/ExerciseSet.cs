namespace TrainingTracker.Client.Server.Models
{
    public class ExerciseSet
    {
        public int Id { get; set; }
        public int SessionExerciseId { get; set; } // Foreign Key do SessionExercise
        public int SetNumber { get; set; }
        public double Weight { get; set; }
        public int Reps { get; set; }
        public DateTime CompletedAt { get; set; } = DateTime.UtcNow;

        // Relacje
        public SessionExercise SessionExercise { get; set; }
    }
}
