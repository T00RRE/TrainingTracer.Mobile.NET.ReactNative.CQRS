namespace TrainingTracker.Client.Server.DTOs.Templates
{
    public class CreateTemplateDto
    {
        public string Name { get; set; }
        public int UserId { get; set; } // Wymagane na wejściu
    }
}