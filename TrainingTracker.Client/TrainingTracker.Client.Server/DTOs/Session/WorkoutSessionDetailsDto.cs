namespace TrainingTracker.Client.Server.DTOs.Sessions
{
    public class WorkoutSessionDetailsDto
    {
        public string NazwaSzablonu { get; set; }
        public DateTime DataTreningu { get; set; }
        public List<SessionExerciseRowDto> Cwiczenia { get; set; }
    }

    public class SessionExerciseRowDto
    {
        public int SessionExerciseId { get; set; }
        public string Cwiczenie { get; set; }
        public int PlanowaneSerie { get; set; }
        public int WykonaneSerie { get; set; }

        public string? Opis { get; set; }
        public double? OstatniCiezar { get; set; } // double, bo taki typ masz w SetDto.cs
    }
}