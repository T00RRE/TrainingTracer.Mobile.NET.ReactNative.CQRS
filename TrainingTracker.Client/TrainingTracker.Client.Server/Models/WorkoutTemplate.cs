namespace TrainingTracker.Client.Server.Models
{
    public class WorkoutTemplate
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public int UserId { get; set; } // Foreign Key
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        // Relacje
        public User User { get; set; } // Właściwość nawigacyjna
        public ICollection<TemplateExercise> TemplateExercises { get; set; }
    }
}
