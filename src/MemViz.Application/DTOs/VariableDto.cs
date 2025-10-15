using MemViz.Domain.Enums;

namespace MemViz.Application.DTOs;

/// <summary>
/// DTO representing a variable
/// </summary>
public record VariableDto
{
    public Guid Id { get; init; }
    public string Name { get; init; } = string.Empty;
    public ValueTypeKind Type { get; init; }
    public string? Value { get; init; }
    public string Address { get; init; } = string.Empty;
    public int Size { get; init; }
    public bool IsInitialized { get; init; }
    public string? PointsTo { get; init; }
}