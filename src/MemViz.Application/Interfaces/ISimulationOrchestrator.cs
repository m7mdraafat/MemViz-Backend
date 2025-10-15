using MemViz.Application.DTOs;
using MemViz.Shared.Results;

namespace MemViz.Application.Interfaces;

/// <summary>
/// Orchestrates the simulation lifecycle.
/// </summary>
public interface ISimulationOrchestrator
{
    /// <summary>
    /// Creates a new simulation from source code.
    /// </summary>
    /// <param name="request">Simulation creation request.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>Result containing simulation response or error.</returns>
    Task<Result<SimulationResponseDto>> CreateSimulationAsync(SimulationRequestDto request, CancellationToken cancellationToken = default);

    /// <summary>
    /// Get simulation details by ID.
    /// </summary>
    /// <param name="simulationId">The ID of the simulation.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>Result containing simulation response or error.</returns>
    Task<Result<SimulationResponseDto>> GetSimulationAsync(Guid simulationId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Starts simulation execution.
    /// </summary>
    /// <param name="simulationId">The ID of the simulation to start.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>Result containing simulation response or error.</returns>
    Task<Result<SimulationResponseDto>> StartSimulationAsync(Guid simulationId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Executes next step in the simulation.
    /// </summary>
    /// <param name="simulationId">The ID of the simulation.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>Result containing updated simulation response or error.</returns>
    Task<Result<SimulationResponseDto>> StepForwardAsync(Guid simulationId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Executes previous step in simulation.
    /// </summary>
    /// <param name="simulationId">The ID of the simulation.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>Result containing updated simulation response or error.</returns>
    Task<Result<SimulationResponseDto>> StepBackwardAsync(Guid simulationId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Pause simulation execution.
    /// </summary>
    /// <param name="simulationId">The ID of the simulation to pause.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>Result containing simulation response or error.</returns>
    Task<Result<SimulationResponseDto>> PauseSimulationAsync(Guid simulationId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Resumes a paused simulation.
    /// </summary>
    /// <param name="simulationId">The ID of the simulation to resume.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>Result containing simulation response or error.</returns>
    Task<Result<SimulationResponseDto>> ResumeSimulationAsync(Guid simulationId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Resets the simulation to the initial state.
    /// </summary>
    /// <param name="simulationId">The ID of the simulation to reset.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>Result containing simulation response or error.</returns>
    Task<Result<SimulationResponseDto>> ResetSimulationAsync(Guid simulationId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Jumps to a specific step in the simulation.
    /// </summary>
    /// <param name="simulationId">The ID of the simulation.</param>
    /// <param name="stepIndex">The target step index to jump to.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>Result containing updated simulation response or error.</returns>
    Task<Result<SimulationResponseDto>> GoToStepAsync(Guid simulationId, int stepIndex, CancellationToken cancellationToken = default);

    /// <summary>
    /// Streams simulation steps as they are executed.
    /// </summary>
    /// <param name="simulationId">The ID of the simulation.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>Async enumerable of simulation steps.</returns>
    IAsyncEnumerable<SimulationStepDto> StreamSimulationStepsAsync(Guid simulationId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets all simulations (for API listing).
    /// </summary>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>Result containing list of simulations.</returns>
    Task<Result<List<SimulationResponseDto>>> GetAllSimulationsAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// Deletes a simulation by ID.
    /// </summary>
    /// <param name="simulationId">The ID of the simulation to delete.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>Result indicating success or failure.</returns>
    Task<Result> DeleteSimulationAsync(Guid simulationId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets current simulation state for streaming.
    /// </summary>
    /// <param name="simulationId">The ID of the simulation.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>Result containing current simulation state.</returns>
    Task<Result<SimulationDto>> GetSimulationStateAsync(Guid simulationId, CancellationToken cancellationToken = default);
}