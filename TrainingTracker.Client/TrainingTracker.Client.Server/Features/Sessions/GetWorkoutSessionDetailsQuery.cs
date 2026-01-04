using MediatR;
using TrainingTracker.Client.Server.DTOs.Sessions;

namespace TrainingTracker.Client.Server.Features.Sessions
{
    // Przyjmujemy SessionId (np. 2) i TemplateId (np. 1), tak jak w Twoim teście SQL
    public record GetWorkoutSessionDetailsQuery(int SessionId, int TemplateId) : IRequest<WorkoutSessionDetailsDto>;
}