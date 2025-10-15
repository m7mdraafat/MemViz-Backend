namespace MemViz.Application.DTOs;

/// <summary>
/// DTO representing a pointer connection
/// </summary>
public record PointerDto
{
    public Guid Id { get; init; }
    public string Name { get; init; } = string.Empty;
    public string SourceAddress { get; init; } = string.Empty;
    public string TargetAddress { get; init; } = string.Empty;
    public string? Label { get; init; }
    public bool IsValid { get; init; }
}