namespace TrainingTracker.Client.Server.DTOs.UserSettings
{
    public class UserSettingsDto
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public string WeightUnit { get; set; }
        public string Theme { get; set; }
    }
}