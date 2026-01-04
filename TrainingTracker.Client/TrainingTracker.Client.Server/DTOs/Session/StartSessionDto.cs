namespace TrainingTracker.Client.Server.DTOs.Sessions
{
    public class StartSessionDto
    {
        public int UserId { get; set; } // Kto rozpoczyna sesję
        public int TemplateId { get; set; }
    }
}