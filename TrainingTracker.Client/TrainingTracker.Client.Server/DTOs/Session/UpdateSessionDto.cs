namespace TrainingTracker.Client.Server.DTOs.Sessions
{
    public class UpdateSessionDto
    {
        public string? Notes { get; set; }
        public bool EndSession { get; set; } = false; // Flaga do jawnego zakończenia sesji
    }
}