using MemViz.Domain.Enums;

namespace MemViz.Application.DTOs;

/// <summary>
/// DTO representing a heap object
/// </summary>
public record HeapObjectDto
{
    public Guid Id { get; init; }
    public string Name { get; init; } = string.Empty;
    public ValueTypeKind Type { get; init; }
    public string Address { get; init; } = string.Empty;
    public int Size { get; init; }
    public bool IsAllocated { get; init; }
    public DateTime AllocatedAt { get; init; }
    public DateTime? DeallocatedAt { get; init; }
    public List<VariableDto> Fields { get; init; } = new();
}