namespace TrainingTracker.Client.Server.Models
{
    public class ExerciseCategory
    {
        public int Id { get; set; }
        public string Name { get; set; } // np. "Chest", "Back", "Legs"

        public ICollection<Exercise> Exercises { get; set; }
    }
}
