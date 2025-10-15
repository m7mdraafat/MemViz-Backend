using MemViz.Domain.Common;

namespace MemViz.Domain.Events;

/// <summary>
/// Raised when a simulation starts execution.
/// </summary>

public record SimulationStartedEvent : DomainEvent
{
    public Guid SimulationId { get; init; }
    public string Language { get; init; } 
    public int TotalSteps { get; init; }

    public SimulationStartedEvent(Guid simulationId, string language, int totalSteps)
    {
        SimulationId = simulationId;
        Language = language;
        TotalSteps = totalSteps;
    }
}