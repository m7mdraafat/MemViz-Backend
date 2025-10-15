using MemViz.Domain.Common;

namespace MemViz.Domain.Events;

/// <summary>
/// Raised when heap memory is deallocated.
/// </summary>
public record HeapDeallocationEvent : DomainEvent
{
    public Guid SimulationId { get; init; }
    public Guid HeapObjectId { get; init; }
    public string ObjectName { get; init; }

    public HeapDeallocationEvent(Guid simulationId, Guid heapObjectId, string objectName)
    {
        SimulationId = simulationId;
        HeapObjectId = heapObjectId;
        ObjectName = objectName;
    }
}