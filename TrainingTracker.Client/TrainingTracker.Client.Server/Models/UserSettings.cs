namespace TrainingTracker.Client.Server.Models
{
    public class UserSettings
    {
        public int Id { get; set; }
        public int UserId { get; set; } // Foreign Key
        public string WeightUnit { get; set; } // "kg" lub "lbs"
        public string Theme { get; set; } // "dark" lub "light"

        public User User { get; set; }
    }
}
