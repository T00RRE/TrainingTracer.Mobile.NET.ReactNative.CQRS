public class TrainingSessionDetailsDto
{
    public int Id { get; set; }
    public string TemplateName { get; set; } = string.Empty;
    public DateTime Date { get; set; }
    public string Duration { get; set; } = string.Empty;
    public string? Notes { get; set; }
    public List<SessionExerciseDetailsDto> Exercises { get; set; } = new();
}

public class SessionExerciseDetailsDto
{
    public string ExerciseName { get; set; } = string.Empty;
    public List<SetDto> Sets { get; set; } = new();
}

public class SetDto
{
    public int Id { get; set; }
    public int SessionExerciseId { get; set; }
    public int SetNumber { get; set; }
    public double Weight { get; set; }
    public int Reps { get; set; }
    public DateTime? CompletedAt { get; set; }
}