namespace TrainingTracker.Client.Server.Models
{
    public class Exercise
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public bool IsGlobal { get; set; }
        public int CategoryId { get; set; } // Foreign Key

        // Relacje
        public ExerciseCategory Category { get; set; }
        public ICollection<TemplateExercise> TemplateExercises { get; set; }
    }
}
