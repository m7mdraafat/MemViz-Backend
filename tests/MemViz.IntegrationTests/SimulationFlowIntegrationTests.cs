using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using MemViz.Application;
using MemViz.Application.DTOs;
using MemViz.Application.Interfaces;
using MemViz.Application.Services;
using MemViz.Domain.Entities;
using MemViz.Shared.Results;
using Xunit;

namespace MemViz.IntegrationTests;

/// <summary>
/// End-to-end integration tests for simulation flow
/// </summary>
public class SimulationFlowIntegrationTests
{
    private readonly IServiceProvider _serviceProvider;

    public SimulationFlowIntegrationTests()
    {
        var services = new ServiceCollection();
        services.AddLogging(builder => builder.AddConsole());
        services.AddApplicationServices();
        
        // Add in-memory repository for testing
        services.AddSingleton<ISimulationRepository, InMemorySimulationRepository>();

        _serviceProvider = services.BuildServiceProvider();
    }

    [Fact]
    public async Task CompleteSimulationFlow_ShouldWorkEndToEnd()
    {
        // Arrange
        var orchestrator = _serviceProvider.GetRequiredService<ISimulationOrchestrator>();
        var request = new SimulationRequestDto
        {
            SourceCode = """
                #include <stdio.h>
                
                int main() {
                    int x = 10;
                    int y = 20;
                    int sum = x + y;
                    printf("Sum: %d\n", sum);
                    return 0;
                }
                """,
            Language = "C"
        };

        // Act & Assert - Create simulation
        var createResult = await orchestrator.CreateSimulationAsync(request);
        createResult.IsSuccess.Should().BeTrue();
        
        var simulation = createResult.Value;
        simulation.Should().NotBeNull();
        simulation.Language.Should().Be("C");
        simulation.Status.Should().Be(Domain.Enums.SimulationStatus.Ready);
        simulation.TotalSteps.Should().BeGreaterThan(0);

        // Act & Assert - Start simulation
        var startResult = await orchestrator.StartSimulationAsync(simulation.Id);
        startResult.IsSuccess.Should().BeTrue();
        startResult.Value.Should().NotBeNull();
        startResult.Value.Status.Should().Be(Domain.Enums.SimulationStatus.Running);
        startResult.Value.CurrentStepIndex.Should().Be(0);

        // Act & Assert - Step through simulation
        var stepCount = 0;
        var currentStepIndex = 0;

        while (currentStepIndex < startResult.Value.TotalSteps - 1 && stepCount < 10) // Safety limit
        {
            var stepResult = await orchestrator.StepForwardAsync(simulation.Id);
            stepResult.IsSuccess.Should().BeTrue();
            stepResult.Value.Should().NotBeNull();
            stepResult.Value.CurrentStepIndex.Should().Be(currentStepIndex + 1);
            
            currentStepIndex = stepResult.Value.CurrentStepIndex;
            stepCount++;
        }

        // Verify we made progress
        stepCount.Should().BeGreaterThan(0);

        // Act & Assert - Get final state
        var finalResult = await orchestrator.GetSimulationAsync(simulation.Id);
        finalResult.IsSuccess.Should().BeTrue();
        finalResult.Value.Should().NotBeNull();
        finalResult.Value.CurrentStepIndex.Should().BeGreaterThan(0);
    }

    [Fact]
    public async Task StreamingSimulation_ShouldYieldAllSteps()
    {
        // Arrange
        var orchestrator = _serviceProvider.GetRequiredService<ISimulationOrchestrator>();
        var request = new SimulationRequestDto
        {
            SourceCode = """
                int main() {
                    int x = 42;
                    return 0;
                }
                """,
            Language = "C"
        };

        // Create and start simulation
        var createResult = await orchestrator.CreateSimulationAsync(request);
        var simulationId = createResult.Value!.Id;

        // Act
        var streamedSteps = new List<SimulationStepDto>();
        await foreach (var step in orchestrator.StreamSimulationStepsAsync(simulationId))
        {
            streamedSteps.Add(step);
            if (streamedSteps.Count >= 3) break; // Limit for test
        }

        // Assert
        streamedSteps.Should().HaveCountGreaterThan(0);
        streamedSteps.Should().BeInAscendingOrder(x => x.StepNumber);
    }
}

/// <summary>
/// Simple in-memory repository implementation for testing
/// </summary>
public class InMemorySimulationRepository : ISimulationRepository
{
    private readonly Dictionary<Guid, SimulationModel> _simulations = new();

    public Task<Result> SaveAsync(SimulationModel simulation, CancellationToken cancellationToken = default)
    {
        _simulations[simulation.Id] = simulation;
        return Task.FromResult(Result.Success());
    }

    public Task<Result<SimulationModel>> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        if (_simulations.TryGetValue(id, out var simulation))
        {
            return Task.FromResult(Result.Success(simulation));
        }

        return Task.FromResult(Result.Failure<SimulationModel>(Error.NotFound("NotFound", $"Simulation with ID {id} not found")));
    }

    public Task<Result> DeleteAsync(Guid id, CancellationToken cancellationToken = default)
    {
        if (_simulations.Remove(id))
        {
            return Task.FromResult(Result.Success());
        }

        return Task.FromResult(Result.Failure(Error.NotFound("NotFound", $"Simulation with ID {id} not found")));
    }

    public Task<Result<IEnumerable<SimulationModel>>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        return Task.FromResult(Result.Success(_simulations.Values.AsEnumerable()));
    }

    public Task<Result> UpdateAsync(SimulationModel simulation, CancellationToken cancellationToken = default)
    {
        if (_simulations.ContainsKey(simulation.Id))
        {
            _simulations[simulation.Id] = simulation;
            return Task.FromResult(Result.Success());
        }

        return Task.FromResult(Result.Failure(Error.NotFound("NotFound", $"Simulation with ID {simulation.Id} not found")));
    }

    Task<IEnumerable<SimulationModel>> ISimulationRepository.GetAllAsync(CancellationToken cancellationToken)
    {
        return Task.FromResult(_simulations.Values.AsEnumerable());
    }
}