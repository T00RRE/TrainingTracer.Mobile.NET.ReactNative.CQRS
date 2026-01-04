namespace TrainingTracker.Client.Server.DTOs.Sessions
{
    public class SessionDto
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public int TemplateId { get; set; }
        public DateTime StartedAt { get; set; }
        public DateTime? CompletedAt { get; set; } // Może być null, jeśli sesja trwa
        public string Notes { get; set; }
        public string TemplateName { get; set; } = string.Empty;
        public TimeSpan? Duration => CompletedAt.HasValue ? CompletedAt.Value - StartedAt : (TimeSpan?)null;
    }
}