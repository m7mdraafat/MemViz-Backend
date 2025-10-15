namespace MemViz.Application.DTOs;

/// <summary>
/// Request DTO for creating a new simulation.
/// </summary>
public record SimulationRequestDto
{
    public string SourceCode { get; init; } = string.Empty;
    public string Language { get; init; } = string.Empty;
}