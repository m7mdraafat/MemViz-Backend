using MemViz.Domain.Common;
using MemViz.Domain.ValueObjects;

namespace MemViz.Domain.Events;

/// <summary>
/// Raised when a new stack frame is created (function call)
/// </summary>

public record StackFrameCreatedEvent : DomainEvent
{
    public Guid SimulationId { get; init; }
    public Guid StackFrameId { get; init; }
    public string FunctionName { get; init; }
    public MemoryAddress BaseAddress { get; init; }
    public int LineNumber { get; init; }

    public StackFrameCreatedEvent(
        Guid simulationId, 
        Guid stackFrameId, 
        string functionName, 
        MemoryAddress baseAddress, 
        int lineNumber)
    {
        SimulationId = simulationId;
        StackFrameId = stackFrameId;
        FunctionName = functionName;
        BaseAddress = baseAddress;
        LineNumber = lineNumber;
    }
}