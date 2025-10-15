using MemViz.Domain.Common;
using MemViz.Domain.Enums;
using MemViz.Domain.Events;

namespace MemViz.Domain.Entities;

/// <summary>
/// Root aggregate representing the entire memory simulation.
/// </summary>
public class SimulationModel : Entity
{
    public string SourceCode { get; private set; }
    public string Language { get; private set; }
    public SimulationStatus Status { get; private set; }
    public int CurrentStepIndex { get; private set; }
    public DateTime CreatedAt { get; private set; }
    public DateTime? StartedAt { get; private set; }
    public DateTime? CompletedAt { get; private set; }
    public string? ErrorMessage { get; private set; }

    private readonly List<SimulationStep> _steps;
    public IReadOnlyList<SimulationStep> Steps => _steps.AsReadOnly();

    public int TotalSteps => _steps.Count;
    public SimulationStep? CurrentStep => (CurrentStepIndex >= 0 && CurrentStepIndex < _steps.Count)
        ? _steps[CurrentStepIndex]
        : null;

    private SimulationModel() : base()
    {
        _steps = new List<SimulationStep>();
    }

    public SimulationModel(string sourceCode, string language)
    {
        if (string.IsNullOrWhiteSpace(sourceCode))
            throw new ArgumentException("Source code cannot be null or empty", nameof(sourceCode));

        if (string.IsNullOrWhiteSpace(language))
            throw new ArgumentException("Language cannot be null or empty", nameof(language));

        SourceCode = sourceCode;
        Language = language;
        Status = SimulationStatus.Ready;
        CurrentStepIndex = -1;
        CreatedAt = DateTime.UtcNow;
        StartedAt = null;
        CompletedAt = null;
        ErrorMessage = null;
        _steps = new List<SimulationStep>();
    }

    public void AddStep(SimulationStep step)
    {
        if (Status == SimulationStatus.Completed)
            throw new InvalidOperationException("Cannot add steps to a completed simulation.");

        _steps.Add(step);
    }

    public void AddSteps(IEnumerable<SimulationStep> steps)
    {
        foreach (var step in steps)
        {
            AddStep(step);
        }
    }
    
    public void Start()
    {
        if (Status != SimulationStatus.Ready && Status != SimulationStatus.Reset)
            throw new InvalidOperationException($"Cannot start simulation in {Status} status");
        
        if (_steps.Count == 0)
            throw new InvalidOperationException("Cannot start simulation with no steps");
        
        Status = SimulationStatus.Running;
        StartedAt = DateTime.UtcNow;
        CurrentStepIndex = 0;
        
        // Raise domain event
        RaiseDomainEvent(new SimulationStartedEvent(Id, Language, TotalSteps));
    }
    
    public void StepForward()
    {
        if (Status != SimulationStatus.Running && Status != SimulationStatus.Paused)
            throw new InvalidOperationException($"Cannot step forward in {Status} status");
        
        if (CurrentStepIndex >= _steps.Count - 1)
        {
            Complete();
            return;
        }
        
        CurrentStepIndex++;
        Status = SimulationStatus.Running;
        
        // Raise step completed event if we have a current step
        if (CurrentStep != null)
        {
            RaiseDomainEvent(new SimulationStepCompletedEvent(
                Id,
                CurrentStep.StepNumber,
                CurrentStep.LineNumber,
                CurrentStep.Operation,
                CurrentStep.Description,
                CurrentStep.StackFrames.Count,
                CurrentStep.HeapObjects.Count,
                CurrentStep.Pointers.Count));
        }
    }
    
    public void StepBackward()
    {
        if (Status != SimulationStatus.Running && Status != SimulationStatus.Paused)
            throw new InvalidOperationException($"Cannot step backward in {Status} status");
        
        if (CurrentStepIndex <= 0)
            throw new InvalidOperationException("Already at the first step");
        
        CurrentStepIndex--;
        Status = SimulationStatus.Running;
    }
    
    public void Pause()
    {
        if (Status != SimulationStatus.Running)
            throw new InvalidOperationException($"Cannot pause simulation in {Status} status");
        
        Status = SimulationStatus.Paused;
    }
    
    public void Resume()
    {
        if (Status != SimulationStatus.Paused)
            throw new InvalidOperationException($"Cannot resume simulation in {Status} status");
        
        Status = SimulationStatus.Running;
    }
    
    public void Complete()
    {
        if (Status == SimulationStatus.Completed)
            return;
        
        var previousStatus = Status;
        Status = SimulationStatus.Completed;
        CompletedAt = DateTime.UtcNow;
        
        // Calculate execution time
        var executionTime = StartedAt.HasValue 
            ? CompletedAt.Value - StartedAt.Value 
            : TimeSpan.Zero;
        
        // Raise completion event
        RaiseDomainEvent(new SimulationCompletedEvent(
            Id, 
            Status, 
            executionTime, 
            Math.Max(0, CurrentStepIndex + 1)));
    }
    
    public void Reset()
    {
        Status = SimulationStatus.Reset;
        CurrentStepIndex = -1;
        StartedAt = null;
        CompletedAt = null;
        ErrorMessage = null;
    }
    
    public void SetError(string errorMessage)
    {
        var previousStatus = Status;
        Status = SimulationStatus.Error;
        ErrorMessage = errorMessage;
        CompletedAt = DateTime.UtcNow;
        
        // Calculate execution time
        var executionTime = StartedAt.HasValue 
            ? CompletedAt.Value - StartedAt.Value 
            : TimeSpan.Zero;
        
        // Raise completion event with error
        RaiseDomainEvent(new SimulationCompletedEvent(
            Id, 
            Status, 
            executionTime, 
            Math.Max(0, CurrentStepIndex + 1),
            errorMessage));
    }
    
    public void GoToStep(int stepIndex)
    {
        if (stepIndex < 0 || stepIndex >= _steps.Count)
            throw new ArgumentOutOfRangeException(nameof(stepIndex), "Step index is out of range");
        
        CurrentStepIndex = stepIndex;
        
        if (Status == SimulationStatus.Ready || Status == SimulationStatus.Reset)
        {
            Status = SimulationStatus.Running;
            StartedAt ??= DateTime.UtcNow;
            
            // Raise started event if this is the first transition to running
            if (StartedAt.Value == DateTime.UtcNow)
            {
                RaiseDomainEvent(new SimulationStartedEvent(Id, Language, TotalSteps));
            }
        }
        
        // Raise step completed event for the target step
        var targetStep = _steps[stepIndex];
        RaiseDomainEvent(new SimulationStepCompletedEvent(
            Id,
            targetStep.StepNumber,
            targetStep.LineNumber,
            targetStep.Operation,
            targetStep.Description,
            targetStep.StackFrames.Count,
            targetStep.HeapObjects.Count,
            targetStep.Pointers.Count));
    }
}