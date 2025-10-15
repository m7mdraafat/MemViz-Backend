namespace MemViz.Application.DTOs;

/// <summary>
/// DTO representing a stack frame
/// </summary>
public record StackFrameDto
{
    public Guid Id { get; init; }
    public string FunctionName { get; init; } = string.Empty;
    public string BaseAddress { get; init; } = string.Empty;
    public int Size { get; init; }
    public int LineNumber { get; init; }
    public DateTime CreatedAt { get; init; }
    public List<VariableDto> Variables { get; init; } = new();
}