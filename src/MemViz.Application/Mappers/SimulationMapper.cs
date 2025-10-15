using MemViz.Application.DTOs;
using MemViz.Domain.Entities;

namespace MemViz.Application.Mappers;

/// <summary>
/// Mapper for converting between simulation domain models and DTOs
/// </summary>
public static class SimulationMapper
{
    public static SimulationResponseDto ToDto(SimulationModel simulation)
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
            Steps = simulation.Steps.Select(ToStepDto).ToList()
        };
    }

    public static SimulationStepDto ToStepDto(SimulationStep step)
    {
        return new SimulationStepDto
        {
            Id = step.Id,
            StepNumber = step.StepNumber,
            LineNumber = step.LineNumber,
            CodeLine = step.CodeLine,
            Operation = step.Operation,
            Description = step.Description,
            ExecutedAt = step.ExecutedAt,
            StackFrames = step.StackFrames.Select(ToStackFrameDto).ToList(),
            HeapObjects = step.HeapObjects.Select(ToHeapObjectDto).ToList(),
            Pointers = step.Pointers.Select(ToPointerDto).ToList()
        };
    }

    public static StackFrameDto ToStackFrameDto(StackFrame stackFrame)
    {
        return new StackFrameDto
        {
            Id = stackFrame.Id,
            FunctionName = stackFrame.FunctionName,
            BaseAddress = stackFrame.BaseAddress.Value,
            Size = stackFrame.Size,
            LineNumber = stackFrame.LineNumber,
            CreatedAt = stackFrame.CreatedAt,
            Variables = stackFrame.Variables.Select(ToVariableDto).ToList()
        };
    }

    public static VariableDto ToVariableDto(Variable variable)
    {
        return new VariableDto
        {
            Id = variable.Id,
            Name = variable.Name,
            Type = variable.Type,
            Value = variable.Value,
            Address = variable.Address.Value,
            Size = variable.Size,
            IsInitialized = variable.IsInitialized,
            PointsTo = variable.PointsTo?.Value
        };
    }

    public static HeapObjectDto ToHeapObjectDto(HeapObject heapObject)
    {
        return new HeapObjectDto
        {
            Id = heapObject.Id,
            Name = heapObject.Name,
            Type = heapObject.Type,
            Address = heapObject.Address.Value,
            Size = heapObject.Size,
            IsAllocated = heapObject.IsAllocated,
            AllocatedAt = heapObject.AllocatedAt,
            DeallocatedAt = heapObject.DeallocatedAt,
            Fields = heapObject.Fields.Select(ToVariableDto).ToList()
        };
    }

    public static PointerDto ToPointerDto(Pointer pointer)
    {
        return new PointerDto
        {
            Id = pointer.Id,
            Name = pointer.Name,
            SourceAddress = pointer.SourceAddress.Value,
            TargetAddress = pointer.TargetAddress.Value,
            Label = pointer.Label,
            IsValid = pointer.IsValid
        };
    }
}