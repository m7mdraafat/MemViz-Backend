using MemViz.Application.Interfaces;
using MemViz.Domain.Entities;
using MemViz.Shared.Results;

namespace MemViz.Infrastructure.Repositories;

/// <summary>
/// In-memory simulation repository for Phase 6 API integration testing
/// This is a temporary implementation to enable API functionality
/// </summary>
public class InMemorySimulationRepository : ISimulationRepository
{
    private readonly Dictionary<Guid, SimulationModel> _simulations = new();

    public async Task<Result> SaveAsync(SimulationModel simulation, CancellationToken cancellationToken = default)
    {
        _simulations[simulation.Id] = simulation;
        return Result.Success();
    }

    public async Task<Result<SimulationModel>> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        if (_simulations.TryGetValue(id, out var simulation))
        {
            return Result.Success(simulation);
        }
        
        return Result.Failure<SimulationModel>(Error.NotFound("SIMULATION_NOT_FOUND", $"Simulation with ID {id} not found"));
    }

    public async Task<Result> UpdateAsync(SimulationModel simulation, CancellationToken cancellationToken = default)
    {
        if (_simulations.ContainsKey(simulation.Id))
        {
            _simulations[simulation.Id] = simulation;
            return Result.Success();
        }
        
        return Result.Failure(Error.NotFound("SIMULATION_NOT_FOUND", $"Simulation with ID {simulation.Id} not found"));
    }

    public async Task<Result> DeleteAsync(Guid id, CancellationToken cancellationToken = default)
    {
        if (_simulations.Remove(id))
        {
            return Result.Success();
        }
        
        return Result.Failure(Error.NotFound("SIMULATION_NOT_FOUND", $"Simulation with ID {id} not found"));
    }

    public async Task<IEnumerable<SimulationModel>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        return _simulations.Values.ToList();
    }
}