namespace TrainingTracker.Client.Server.Models
{
    public class WorkoutTemplate
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public int UserId { get; set; } 
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public User User { get; set; } 
        public ICollection<TemplateExercise> TemplateExercises { get; set; }
    }
}
