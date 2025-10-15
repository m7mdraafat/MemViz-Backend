using MemViz.Domain.Enums;

namespace MemViz.Application.DTOs;

/// <summary>
/// DTO representing a single simulation step
/// </summary>
public record SimulationStepDto
{
    public Guid Id { get; init; }
    public int StepNumber { get; init; }
    public int LineNumber { get; init; }
    public string CodeLine { get; init; } = string.Empty;
    public OperationType Operation { get; init; }
    public string Description { get; init; } = string.Empty;
    public DateTime ExecutedAt { get; init; }
    public List<StackFrameDto> StackFrames { get; init; } = new();
    public List<HeapObjectDto> HeapObjects { get; init; } = new();
    public List<PointerDto> Pointers { get; init; } = new();
}