namespace TrainingTracker.Client.Server.DTOs.SessionExercises
{
    public class AddSessionExerciseDto
    {
        public int SessionId { get; set; }
        public int ExerciseId { get; set; }
        public int OrderPosition { get; set; }
    }
}