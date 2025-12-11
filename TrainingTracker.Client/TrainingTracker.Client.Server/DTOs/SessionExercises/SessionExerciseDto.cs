namespace TrainingTracker.Client.Server.DTOs.SessionExercises
{
    public class SessionExerciseDto
    {
        public int Id { get; set; }
        public int SessionId { get; set; }
        public int ExerciseId { get; set; }
        public string ExerciseName { get; set; }
        public int OrderPosition { get; set; }
    }
}