using MemViz.Domain.Common;
using MemViz.Domain.Enums;

namespace MemViz.Domain.Entities;

/// <summary>
/// Represents a single step in the simulation execution.
/// </summary>
public class SimulationStep : Entity
{
    public int StepNumber { get; private set; }
    public int LineNumber { get; private set; }
    public string CodeLine { get; private set; }
    public OperationType Operation { get; private set; }
    public string Description { get; private set; }
    public DateTime ExecutedAt { get; private set; }

    private readonly List<StackFrame> _stackFrames;
    private readonly List<HeapObject> _heapObjects;
    private readonly List<Pointer> _pointers;

    public IReadOnlyList<StackFrame> StackFrames => _stackFrames.AsReadOnly();
    public IReadOnlyList<HeapObject> HeapObjects => _heapObjects.AsReadOnly();
    public IReadOnlyList<Pointer> Pointers => _pointers.AsReadOnly();

    private SimulationStep() : base()
    {
        _stackFrames = new List<StackFrame>();
        _heapObjects = new List<HeapObject>();
        _pointers = new List<Pointer>();
    }

    public SimulationStep(
        int stepNumber,
        int lineNumber,
        string codeLine,
        OperationType operation,
        string description)
    {
        if (stepNumber < 0)
            throw new ArgumentException("Step number cannot be negative", nameof(stepNumber));

        if (lineNumber < 0)
            throw new ArgumentException("Line number cannot be negative", nameof(lineNumber));

        if (string.IsNullOrWhiteSpace(codeLine))
            throw new ArgumentException("Code line cannot be null or empty", nameof(codeLine));

        StepNumber = stepNumber;
        LineNumber = lineNumber;
        CodeLine = codeLine;
        Operation = operation;
        Description = description ?? string.Empty;
        ExecutedAt = DateTime.UtcNow;

        _stackFrames = new List<StackFrame>();
        _heapObjects = new List<HeapObject>();
        _pointers = new List<Pointer>();
    }

    public void SetStackFrames(IEnumerable<StackFrame> stackFrames)
    {
        _stackFrames.Clear();
        _stackFrames.AddRange(stackFrames);
    }

    public void SetHeapObjects(IEnumerable<HeapObject> heapObjects)
    {
        _heapObjects.Clear();
        _heapObjects.AddRange(heapObjects);
    }

    public void SetPointers(IEnumerable<Pointer> pointers)
    {
        _pointers.Clear();
        _pointers.AddRange(pointers);
    }

    public void AddStackFrame(StackFrame stackFrame)
    {
        _stackFrames.Add(stackFrame);
    }

    public void AddHeapObject(HeapObject heapObject)
    {
        _heapObjects.Add(heapObject);
    }

    public void AddPointer(Pointer pointer)
    {
        _pointers.Add(pointer);
    }
}