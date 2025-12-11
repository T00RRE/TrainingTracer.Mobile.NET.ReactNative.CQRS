namespace TrainingTracker.Client.Server.DTOs.Exercises
{
    public class ExerciseDto
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public bool IsGlobal { get; set; }
        public string CategoryName { get; set; }
    }
}