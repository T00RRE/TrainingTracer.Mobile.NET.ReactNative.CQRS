namespace TrainingTracker.Client.Server.DTOs.ExerciseSets
{
    public class AddSetDto
    {
        public int SessionExerciseId { get; set; }
        public int SetNumber { get; set; }
        public double Weight { get; set; }
        public int Reps { get; set; }
    }
}