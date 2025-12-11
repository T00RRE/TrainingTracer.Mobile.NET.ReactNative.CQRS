namespace TrainingTracker.Client.Server.DTOs.ExerciseSets
{
    public class SetDto
    {
        public int Id { get; set; }
        public int SessionExerciseId { get; set; }
        public int SetNumber { get; set; }
        public double Weight { get; set; }
        public int Reps { get; set; }
        public DateTime CompletedAt { get; set; }
    }
}