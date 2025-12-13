namespace TrainingTracker.Client.Server.Models
{
        public class TrainingSession
        {
            public int Id { get; set; }
            public int UserId { get; set; } 
            public DateTime StartedAt { get; set; } = DateTime.UtcNow;
            public DateTime? CompletedAt { get; set; } 
            public string Notes { get; set; }

            public User User { get; set; }
            public ICollection<SessionExercise> SessionExercises { get; set; }
        }
}
