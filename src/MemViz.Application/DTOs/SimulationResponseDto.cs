using MemViz.Domain.Enums;

namespace MemViz.Application.DTOs;

/// <summary>
/// Response DTO containing simulation details.
/// </summary>
public record SimulationResponseDto
{
    public Guid Id { get; init; }
    public string SourceCode { get; init; } = string.Empty;
    public string Language { get; init; } = string.Empty;
    public SimulationStatus Status { get; init; }
    public int CurrentStepIndex { get; init; }
    public int TotalSteps { get; init; }
    public DateTime CreatedAt { get; init; }
    public DateTime? StartedAt { get; init; }
    public DateTime? CompletedAt { get; init; }
    public string? ErrorMessage { get; init; }
    public List<SimulationStepDto> Steps { get; init; } = new();
}