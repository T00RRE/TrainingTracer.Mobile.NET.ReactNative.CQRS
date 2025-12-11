namespace TrainingTracker.Client.Server.Models
{
        public class TrainingSession
        {
            public int Id { get; set; }
            public int UserId { get; set; } // Foreign Key
            public DateTime StartedAt { get; set; } = DateTime.UtcNow;
            public DateTime? CompletedAt { get; set; } // Użycie '?' oznacza, że może być null (sesja trwa)
            public string Notes { get; set; }

            // Relacje
            public User User { get; set; }
            public ICollection<SessionExercise> SessionExercises { get; set; }
        }
}
