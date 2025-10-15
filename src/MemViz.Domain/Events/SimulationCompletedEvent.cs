using MemViz.Domain.Common;
using MemViz.Domain.Enums;

namespace MemViz.Domain.Events;

/// <summary>
/// Raised when a simulation completes (successfully or with errors)
/// </summary>

public record SimulationCompletedEvent : DomainEvent
{
    public Guid SimulationId { get; init; }
    public SimulationStatus FinalStatus { get; init; }
    public string? ErrorMessage { get; init; }
    public TimeSpan ExecutionTime { get; init; }
    public int TotalStepsExecuted { get; init; }

    public SimulationCompletedEvent(
        Guid simulationId, 
        SimulationStatus finalStatus, 
        TimeSpan executionTime, 
        int totalStepsExecuted,
        string? errorMessage = null)
    {
        SimulationId = simulationId;
        FinalStatus = finalStatus;
        ErrorMessage = errorMessage;
        ExecutionTime = executionTime;
        TotalStepsExecuted = totalStepsExecuted;
    }
}