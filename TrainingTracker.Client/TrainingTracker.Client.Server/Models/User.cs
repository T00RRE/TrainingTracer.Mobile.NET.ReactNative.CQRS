namespace TrainingTracker.Client.Server.Models
{
    public class User
    {
        public int Id { get; set; }
        public string Username { get; set; }
        public string Email { get; set; }
        public string PasswordHash { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public ICollection<WorkoutTemplate> Templates { get; set; }
        public ICollection<TrainingSession> Sessions { get; set; }
        public UserSettings Settings { get; set; }
    }
}
