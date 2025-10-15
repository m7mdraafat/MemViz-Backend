using MemViz.Shared.Results;

namespace MemViz.Application.DTOs;

/// <summary>
/// Complete simulation data transfer object for API responses
/// </summary>
public class SimulationDto
{
    /// <summary>
    /// Unique identifier for the simulation
    /// </summary>
    public Guid Id { get; set; }

    /// <summary>
    /// Name or title of the simulation
    /// </summary>
    public string Name { get; set; } = string.Empty;

    /// <summary>
    /// Programming language being simulated
    /// </summary>
    public string Language { get; set; } = string.Empty;

    /// <summary>
    /// Source code being executed
    /// </summary>
    public string SourceCode { get; set; } = string.Empty;

    /// <summary>
    /// Current execution status
    /// </summary>
    public string Status { get; set; } = string.Empty;

    /// <summary>
    /// Current step number in the simulation
    /// </summary>
    public int CurrentStep { get; set; }

    /// <summary>
    /// Total number of steps in the simulation
    /// </summary>
    public int TotalSteps { get; set; }

    /// <summary>
    /// List of execution steps with memory state
    /// </summary>
    public List<SimulationStepDto> Steps { get; set; } = new();

    /// <summary>
    /// Current memory state (latest step)
    /// </summary>
    public SimulationStepDto? CurrentState { get; set; }

    /// <summary>
    /// Creation timestamp
    /// </summary>
    public DateTime CreatedAt { get; set; }

    /// <summary>
    /// Last update timestamp
    /// </summary>
    public DateTime UpdatedAt { get; set; }

    /// <summary>
    /// Optional error information if simulation failed
    /// </summary>
    public string? ErrorMessage { get; set; }

    /// <summary>
    /// Simulation metadata and configuration
    /// </summary>
    public Dictionary<string, object> Metadata { get; set; } = new();
}