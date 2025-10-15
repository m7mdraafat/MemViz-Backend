using FluentValidation;
using MemViz.Application.DTOs;
using MemViz.Application.Interfaces;
using MemViz.Application.Mappers;
using MemViz.Domain.Entities;
using MemViz.Shared.Results;
using Microsoft.Extensions.Logging;

namespace MemViz.Application.Services;

/// <summary>
/// Orchestrates simulation creating, execution, and management.
/// </summary>
public class SimulationOrchestrator : ISimulationOrchestrator
{
    private readonly ISimulationRepository _simulationRepository;
    private readonly IValidator<SimulationRequestDto> _validator;
    private readonly ILogger<SimulationOrchestrator> _logger;

    public SimulationOrchestrator(
        ISimulationRepository simulationRepository,
        IValidator<SimulationRequestDto> requestValidator,
        ILogger<SimulationOrchestrator> logger)
    {
        _simulationRepository = simulationRepository;
        _validator = requestValidator;
        _logger = logger;
    }

    public async Task<Result<SimulationResponseDto>> GetSimulationAsync(Guid simulationId, CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("SimulationOrchestrator - Getting simulation {SimulationId}", simulationId);

        try
        {
            var result = await _simulationRepository.GetByIdAsync(simulationId, cancellationToken);
            if (result.IsFailure)
            {
                _logger.LogWarning("Simulation {SimulationId} not found", simulationId);
                return Result.Failure<SimulationResponseDto>(Error.NotFound("SIMULATION_NOT_FOUND", $"Simulation with ID '{simulationId}' not found."));
            }

            var responseDto = SimulationMapper.ToDto(result.Value!);
            return Result.Success(responseDto);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Exception occurred while retrieving simulation {SimulationId}", simulationId);
            return Result.Failure<SimulationResponseDto>(Error.Unexpected("EXCEPTION", "An unexpected error occurred."));
        }
    }

    public async Task<Result<SimulationResponseDto>> StartSimulationAsync(Guid simulationId, CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("SimulationOrchestrator - Starting simulation {SimulationId}", simulationId);

        return await ExecuteSimulationOperation(simulationId, simulation =>
        {
            simulation.Start();
            return Task.CompletedTask;
        }, "start", cancellationToken);
    }

    public async Task<Result<SimulationResponseDto>> StepForwardAsync(Guid simulationId, CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("SimulationOrchestrator - Stepping forward simulation {SimulationId}", simulationId);

        return await ExecuteSimulationOperation(simulationId, simulation =>
        {
            simulation.StepForward();
            return Task.CompletedTask;
        }, "step forward", cancellationToken);
    }

    public async Task<Result<SimulationResponseDto>> StepBackwardAsync(Guid simulationId, CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("SimulationOrchestrator - Stepping backward simulation {SimulationId}", simulationId);

        return await ExecuteSimulationOperation(simulationId, simulation =>
        {
            simulation.StepBackward();
            return Task.CompletedTask;
        }, "step backward", cancellationToken);
    }

    public async Task<Result<SimulationResponseDto>> PauseSimulationAsync(Guid simulationId, CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("SimulationOrchestrator - Pausing simulation {SimulationId}", simulationId);

        return await ExecuteSimulationOperation(simulationId, simulation =>
        {
            simulation.Pause();
            return Task.CompletedTask;
        }, "pause", cancellationToken);
    }

    public async Task<Result<SimulationResponseDto>> ResumeSimulationAsync(Guid simulationId, CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("SimulationOrchestrator - Resuming simulation {SimulationId}", simulationId);

        return await ExecuteSimulationOperation(simulationId, simulation =>
        {
            simulation.Resume();
            return Task.CompletedTask;
        }, "resume", cancellationToken);
    }

    public async Task<Result<SimulationResponseDto>> ResetSimulationAsync(
    Guid simulationId,
    CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Resetting simulation {SimulationId}", simulationId);

        return await ExecuteSimulationOperation(simulationId, simulation =>
        {
            simulation.Reset();
            return Task.CompletedTask;
        }, "reset", cancellationToken);
    }
    
    public async Task<Result<SimulationResponseDto>> GoToStepAsync(Guid simulationId, int stepIndex, CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("SimulationOrchestrator - Going to step {StepIndex} for simulation {SimulationId}", stepIndex, simulationId);

        return await ExecuteSimulationOperation(simulationId, simulation =>
        {
            simulation.GoToStep(stepIndex);
            return Task.CompletedTask;
        }, "go to step", cancellationToken);
    }

    public async IAsyncEnumerable<SimulationStepDto> StreamSimulationStepsAsync(
        Guid simulationId,
        [System.Runtime.CompilerServices.EnumeratorCancellation] CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("SimulationOrchestrator - Streaming steps for simulation {SimulationId}", simulationId);

        var simulationResult = await _simulationRepository.GetByIdAsync(simulationId, cancellationToken);
        if (simulationResult.IsFailure)
        {
            _logger.LogWarning("SimulationOrchestrator - Simulation {SimulationId} not found for streaming", simulationId);
            yield break;
        }

        var simulation = simulationResult.Value!;

        // Stream current state first
        if (simulation.CurrentStep != null)
        {
            yield return SimulationMapper.ToStepDto(simulation.CurrentStep);
        }

        // Stream remaining steps as they execute
        while (simulation.CurrentStepIndex < simulation.TotalSteps - 1 && !cancellationToken.IsCancellationRequested)
        {
            SimulationStepDto? stepDto = null;
            try
            {
                simulation.StepForward();
                var updateResult = await _simulationRepository.UpdateAsync(simulation, cancellationToken);

                if (simulation.CurrentStep != null)
                {
                    stepDto = SimulationMapper.ToStepDto(simulation.CurrentStep);
                }

                // Simulate delay for demonstration purposes
                await Task.Delay(100, cancellationToken);
            }
            catch (InvalidOperationException ex)
            {
                _logger.LogWarning("SimulationOrchestrator - Cannot continue streaming simulation {SimulationId}: {Error}", simulationId, ex.Message);
                break;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "SimulationOrchestrator - Unexpected error during streaming of simulation {SimulationId}",
                    simulationId);
                break;
            }

            if (stepDto != null)
            {
                yield return stepDto;
            }
        }
        
        _logger.LogInformation("SimulationOrchestrator - Finished streaming steps for simulation {SimulationId}", simulationId);
    }

    /// <summary>
    /// Helper method to execute a simulation operations with consistent error handling.
    /// </summary>
    private async Task<Result<SimulationResponseDto>> ExecuteSimulationOperation(
        Guid simulationId,
        Func<SimulationModel, Task> operation,
        string operationName,
        CancellationToken cancellationToken = default)
    {
        try
        {
            var result = await _simulationRepository.GetByIdAsync(simulationId, cancellationToken);
            if (result.IsFailure)
            {
                return Result.Failure<SimulationResponseDto>(result.Error);
            }

            var simulation = result.Value!;
            await operation(simulation);

            var updateResult = await _simulationRepository.UpdateAsync(simulation, cancellationToken);
            if (updateResult.IsFailure)
            {
                _logger.LogError("Failed to update simulation {SimulationId} after {Operation}: {Error}",
                    simulationId, operationName, updateResult.Error);
                return Result.Failure<SimulationResponseDto>(Error.Persistence("UPDATE_FAILED", "Failed to update simulation."));
            }

            var responseDto = SimulationMapper.ToDto(simulation);
            return Result.Success(responseDto);
        }
        catch (InvalidOperationException ex)
        {
            _logger.LogWarning("Invalid operation on simulation {SimulationId} during {Operation}: {Error}",
                simulationId, operationName, ex.Message);
            return Result.Failure<SimulationResponseDto>(Error.Failure("INVALID_OPERATION", "An invalid operation occurred."));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Unexpected error during {Operation} on simulation {SimulationId}",
                operationName, simulationId);
            return Result.Failure<SimulationResponseDto>(Error.Unexpected("EXCEPTION", "An unexpected error occurred."));
        }
    }

    /// <summary>
    /// Gets all simulations (for API listing).
    /// </summary>
    public async Task<Result<List<SimulationResponseDto>>> GetAllSimulationsAsync(CancellationToken cancellationToken = default)
    {
        try
        {
            var simulations = await _simulationRepository.GetAllAsync(cancellationToken);
            var simulationDtos = simulations.Select(SimulationMapper.ToDto).ToList();
            return Result<List<SimulationResponseDto>>.Success(simulationDtos);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to get all simulations");
            return Result.Failure<List<SimulationResponseDto>>(Error.Unexpected("GET_ALL_FAILED", "Failed to retrieve simulations"));
        }
    }

    /// <summary>
    /// Deletes a simulation by ID.
    /// </summary>
    public async Task<Result> DeleteSimulationAsync(Guid simulationId, CancellationToken cancellationToken = default)
    {
        try
        {
            var result = await _simulationRepository.DeleteAsync(simulationId, cancellationToken);
            if (result.IsFailure)
            {
                return Result.Failure(result.Error);
            }

            _logger.LogInformation("Deleted simulation {SimulationId}", simulationId);
            return Result.Success();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to delete simulation {SimulationId}", simulationId);
            return Result.Failure(Error.Unexpected("DELETE_FAILED", "Failed to delete simulation"));
        }
    }

    /// <summary>
    /// Gets current simulation state for streaming.
    /// </summary>
    public async Task<Result<SimulationDto>> GetSimulationStateAsync(Guid simulationId, CancellationToken cancellationToken = default)
    {
        try
        {
            var result = await _simulationRepository.GetByIdAsync(simulationId, cancellationToken);
            if (result.IsFailure)
            {
                return Result.Failure<SimulationDto>(result.Error);
            }

            var simulation = result.Value!;

            // Create basic simulation DTO with available properties
            var simulationDto = new SimulationDto
            {
                Id = simulation.Id,
                Name = "Simulation", // Default name for now
                Language = simulation.Language,
                SourceCode = simulation.SourceCode,
                Status = simulation.Status.ToString(),
                CurrentStep = 0, // Will be updated when domain models are fixed
                TotalSteps = simulation.Steps.Count,
                Steps = new List<SimulationStepDto>(), // Simplified for now
                CreatedAt = simulation.CreatedAt,
                UpdatedAt = DateTime.UtcNow // Default for now
            };

            return Result.Success(simulationDto);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to get simulation state {SimulationId}", simulationId);
            return Result.Failure<SimulationDto>(Error.Unexpected("GET_STATE_FAILED", "Failed to get simulation state"));
        }
    }
    
        private SimulationResponseDto MapToResponseDto(SimulationModel simulation)
    {
        return new SimulationResponseDto
        {
            Id = simulation.Id,
            SourceCode = simulation.SourceCode,
            Language = simulation.Language,
            Status = simulation.Status,
            CurrentStepIndex = simulation.CurrentStepIndex,
            TotalSteps = simulation.TotalSteps,
            CreatedAt = simulation.CreatedAt,
            StartedAt = simulation.StartedAt,
            CompletedAt = simulation.CompletedAt,
            ErrorMessage = simulation.ErrorMessage,
            Steps = simulation.Steps.Select(MapToStepDto).ToList()
        };
    }

    private SimulationStepDto MapToStepDto(SimulationStep step)
    {
        return new SimulationStepDto
        {
            Id = step.Id,
            StepNumber = step.StepNumber,
            LineNumber = step.LineNumber,
            CodeLine = step.CodeLine,
            Operation = step.Operation,
            Description = step.Description,
            StackFrames = step.StackFrames.Select(MapToStackFrameDto).ToList(),
            HeapObjects = step.HeapObjects.Select(MapToHeapObjectDto).ToList(),
            Pointers = step.Pointers.Select(MapToPointerDto).ToList()
        };
    }

    private StackFrameDto MapToStackFrameDto(StackFrame frame)
    {
        return new StackFrameDto
        {
            Id = frame.Id,
            FunctionName = frame.FunctionName,
            BaseAddress = frame.BaseAddress.ToString(),
            Size = frame.Size,
            LineNumber = frame.LineNumber,
            CreatedAt = frame.CreatedAt,
            Variables = frame.Variables.Select(MapToVariableDto).ToList()
        };
    }

    private HeapObjectDto MapToHeapObjectDto(HeapObject heapObject)
    {
        return new HeapObjectDto
        {
            Id = heapObject.Id,
            Name = heapObject.Name,
            Type = heapObject.Type,
            Address = heapObject.Address.ToString(),
            Size = heapObject.Size,
            IsAllocated = heapObject.IsAllocated,
            AllocatedAt = heapObject.AllocatedAt,
            DeallocatedAt = heapObject.DeallocatedAt,
            Fields = heapObject.Fields.Select(MapToVariableDto).ToList()
        };
    }

    private PointerDto MapToPointerDto(Pointer pointer)
    {
        return new PointerDto
        {
            Id = pointer.Id,
            SourceAddress = pointer.SourceAddress.ToString(),
            TargetAddress = pointer.TargetAddress.ToString(),
            Name = pointer.Name
        };
    }

    private VariableDto MapToVariableDto(Variable variable)
    {
        return new VariableDto
        {
            Id = variable.Id,
            Name = variable.Name,
            Type = variable.Type,
            Value = variable.Value,
            Address = variable.Address.ToString(),
            Size = variable.Size,
            IsInitialized = variable.IsInitialized,
            PointsTo = variable.PointsTo?.ToString()
        };
    }

    public Task<Result<SimulationResponseDto>> CreateSimulationAsync(SimulationRequestDto request, CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException();
    }
}