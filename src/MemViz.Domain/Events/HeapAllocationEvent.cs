using MemViz.Domain.Common;
using MemViz.Domain.Enums;
using MemViz.Domain.ValueObjects;

namespace MemViz.Domain.Events;

/// <summary>
/// Raised when memory is allocated on the heap.
/// </summary>
public record HeapAllocationEvent : DomainEvent
{
    public Guid SimulationId { get; init; }
    public Guid HeapObjectId { get; init; }
    public string ObjectName { get; init; }
    public ValueTypeKind Type { get; init; }
    public MemoryAddress Address { get; init; }
    public int Size { get; init; }

    public HeapAllocationEvent(
        Guid simulationId, 
        Guid heapObjectId, 
        string objectName, 
        ValueTypeKind type, 
        MemoryAddress address, 
        int size)
    {
        SimulationId = simulationId;
        HeapObjectId = heapObjectId;
        ObjectName = objectName;
        Type = type;
        Address = address;
        Size = size;
    }
}