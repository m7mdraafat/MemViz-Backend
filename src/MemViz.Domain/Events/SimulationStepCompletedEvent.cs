using MemViz.Domain.Common;
using MemViz.Domain.Enums;

namespace MemViz.Domain.Events;

/// <summary>
/// Raised when a simulation step is completed.
/// </summary>

public record SimulationStepCompletedEvent : DomainEvent
{
    public Guid SimulationId { get; init; }
    public int StepNumber { get; init; }
    public int LineNumber { get; init; }
    public OperationType Operation { get; init; }
    public string Discription { get; init; }
    public int StackFrameCount { get; init; }
    public int HeapObjectCount { get; init; }
    public int PointerCount { get; init; }

    public SimulationStepCompletedEvent(
        Guid simulationId,
         int stepNumber,
          int lineNumber,
           OperationType operation,
            string discription,
             int stackFrameCount,
              int heapObjectCount,
               int pointerCount)
    {
        SimulationId = simulationId;
        StepNumber = stepNumber;
        LineNumber = lineNumber;
        Operation = operation;
        Discription = discription;
        StackFrameCount = stackFrameCount;
        HeapObjectCount = heapObjectCount;
        PointerCount = pointerCount;
    }
}