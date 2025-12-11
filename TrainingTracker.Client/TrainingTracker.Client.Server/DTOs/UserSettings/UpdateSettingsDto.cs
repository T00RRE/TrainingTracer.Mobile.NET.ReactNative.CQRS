namespace TrainingTracker.Client.Server.DTOs.UserSettings
{
    public class UpdateSettingsDto
    {
        public string WeightUnit { get; set; } // np. "kg" lub "lbs"
        public string Theme { get; set; }      // np. "dark" lub "light"
    }
}