using MemViz.Domain.Entities;
using MemViz.Shared.Results;

namespace MemViz.Application.Interfaces;

/// <summary>
/// Repository interface for simulation persistence
/// </summary>
public interface ISimulationRepository
{
    /// <summary>
    /// Saves a simulation to storage
    /// </summary>
    /// <param name="simulation">Simulation to save</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Result indicating success or failure</returns>
    Task<Result> SaveAsync(SimulationModel simulation, CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets a simulation by ID
    /// </summary>
    /// <param name="id">Simulation identifier</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Result containing simulation or error</returns>
    Task<Result<SimulationModel>> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);

    /// <summary>
    /// Updates an existing simulation
    /// </summary>
    /// <param name="simulation">Simulation to update</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Result indicating success or failure</returns>
    Task<Result> UpdateAsync(SimulationModel simulation, CancellationToken cancellationToken = default);

    /// <summary>
    /// Deletes a simulation
    /// </summary>
    /// <param name="id">Simulation identifier</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Result indicating success or failure</returns>
    Task<Result> DeleteAsync(Guid id, CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets all simulations (for admin/debugging purposes)
    /// </summary>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Collection of all simulations</returns>
    Task<IEnumerable<SimulationModel>> GetAllAsync(CancellationToken cancellationToken = default);
}