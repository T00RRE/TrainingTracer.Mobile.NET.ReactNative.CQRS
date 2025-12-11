namespace TrainingTracker.Client.Server.DTOs.Exercises
{
    public class CreateExerciseDto
    {
        public string Name { get; set; }
        public string Description { get; set; }
        public bool IsGlobal { get; set; }
        public int CategoryId { get; set; }
    }
}