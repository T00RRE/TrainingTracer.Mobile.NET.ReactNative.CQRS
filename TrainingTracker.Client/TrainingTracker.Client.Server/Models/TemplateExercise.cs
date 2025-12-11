namespace TrainingTracker.Client.Server.Models
{
    public class TemplateExercise
    {
        public int Id { get; set; }
        public int TemplateId { get; set; } // FK do WorkoutTemplate
        public int ExerciseId { get; set; } // FK do Exercise
        public int DefaultSets { get; set; }
        public int DefaultReps { get; set; }

        // Relacje
        public WorkoutTemplate Template { get; set; }
        public Exercise Exercise { get; set; }
    }
}
