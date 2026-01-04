namespace TrainingTracker.Client.Server.DTOs.SessionExercises
{
    public class CreateAndAddExerciseDto
    {
        public int SessionId { get; set; }
        public string Name { get; set; } = string.Empty;
        public string? Description { get; set; }
        public int PlannedSets { get; set; }
    }
}