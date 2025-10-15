
using MemViz.Domain.Entities;
using MemViz.Domain.Enums;
using MemViz.Domain.Events;

namespace MemViz.UnitTests.Domain.Entities;

public class SimulationModelTests
{
    [Fact]
    public void Constructor_WithValidParameters_ShouldCreateSimulation()
    {
        // Arrange
        const string sourceCode = "int main() {return 0; }";
        const string language = "C";

        // Act
        var simulation = new SimulationModel(sourceCode, language);

        // Assert
        Assert.NotNull(simulation);
        Assert.Equal(sourceCode, simulation.SourceCode);
        Assert.Equal(language, simulation.Language);
        Assert.Equal(SimulationStatus.Ready, simulation.Status);
        Assert.Equal(-1, simulation.CurrentStepIndex);
        Assert.Equal(0, simulation.TotalSteps);
        Assert.Equal(simulation.CreatedAt.Date, DateTime.UtcNow.Date);
        Assert.Null(simulation.StartedAt);
        Assert.Null(simulation.CompletedAt);
        Assert.Null(simulation.ErrorMessage);
        Assert.Null(simulation.CurrentStep);
    }

    [Theory]
    [InlineData("", "C")]
    [InlineData("   ", "C")]
    [InlineData(null, "C")]
    [InlineData("code", "")]
    [InlineData("code", "   ")]
    [InlineData("code", null)]
    public void Constructor_WithInvalidParameters_ShouldThrowArgumentException(string sourceCode, string language)
    {
        // Act & Assert
        var action = () => new SimulationModel(sourceCode, language);
        Assert.Throws<ArgumentException>(action);
    }

    [Fact]
    public void AddStep_WithValidStep_ShouldAddStep()
    {
        // Arrange
        var simulation = new SimulationModel("int main() {}", "C");
        var step = new SimulationStep(1, 1, "int main()", OperationType.FunctionCall, "Main function entry");

        // Act
        simulation.AddStep(step);

        // Assert
        Assert.Equal(1, simulation.TotalSteps);
        Assert.Equal(step, simulation.Steps[0]);
    }

    [Fact]
    public void AddStep_WhenSimulationCompleted_ShouldThrowInvalidOperationException()
    {
        // Arrange
        var simulation = new SimulationModel("int main() {}", "C");
        var step = new SimulationStep(1, 1, "int main()", OperationType.FunctionCall, "Main function entry");
        simulation.AddStep(step);
        simulation.Start();
        simulation.Complete();

        // Act & Assert
        var action = () => simulation.AddStep(new SimulationStep(2, 2, "return 0;", OperationType.FunctionReturn, "Return"));
        var exception = Assert.Throws<InvalidOperationException>(action);
        Assert.Contains("Cannot add steps to a completed simulation", exception.Message);
    }

    [Fact]
    public void Start_WithValidState_ShouldStartSimulation()
    {
        // Arrange
        var simulation = new SimulationModel("int main() {}", "C");
        var step = new SimulationStep(1, 1, "int main()", OperationType.FunctionCall, "Main function entry");
        simulation.AddStep(step);

        // Act
        simulation.Start();

        // Assert
        Assert.Equal(SimulationStatus.Running, simulation.Status);
        Assert.NotNull(simulation.StartedAt);
        Assert.Equal(0, simulation.CurrentStepIndex);

        // check domain event was raised.
        Assert.Single(simulation.DomainEvents);
        var domainEvent = simulation.DomainEvents[0] as SimulationStartedEvent;
        Assert.NotNull(domainEvent);
    }

    [Fact]
    public void Start_WithNoSteps_ShouldThrowInvalidOperationException()
    {
        // Arrange
        var simulation = new SimulationModel("int main() {}", "C");

        // Act & Assert
        var action = () => simulation.Start();
        var exception = Assert.Throws<InvalidOperationException>(action);
        Assert.Contains("Cannot start simulation with no steps", exception.Message);
    }

    [Theory]
    [InlineData(SimulationStatus.Running)]
    [InlineData(SimulationStatus.Paused)]
    [InlineData(SimulationStatus.Completed)]
    [InlineData(SimulationStatus.Error)]
    public void Start_WithInvalidStatus_ShouldThrowInvalidOperationException(SimulationStatus status)
    {
        // Arrange
        var simulation = new SimulationModel("int main() {}", "C");
        var step = new SimulationStep(1, 1, "int main()", OperationType.FunctionCall, "Main function entry");
        simulation.AddStep(step);

        // Foruce the simulation into the tests status
        if (status == SimulationStatus.Running || status == SimulationStatus.Paused)
        {
            simulation.Start();
            if (status == SimulationStatus.Paused)
                simulation.Pause();
        }
        else if (status == SimulationStatus.Completed)
        {
            simulation.Start();
            simulation.Complete();
        }
        else if (status == SimulationStatus.Error)
        {
            simulation.Start();
            simulation.SetError("Test error");
        }

        // Act & Assert
        var action = () => simulation.Start();
        var exception = Assert.Throws<InvalidOperationException>(action);
        Assert.Contains($"Cannot start simulation in {status} status", exception.Message);
    }

    [Fact]
    public void StepForward_WithValidState_ShouldAdvanceStep()
    {
        // Arrange
        var simulation = new SimulationModel("int main() {}", "C");
        simulation.AddSteps(new[]
        {
            new SimulationStep(1, 1, "int main()", OperationType.FunctionCall, "Main function entry"),
            new SimulationStep(2, 2, "return 0;", OperationType.FunctionReturn, "Return")
        });
        simulation.Start();
        simulation.ClearDomainEvents();

        // Act
        simulation.StepForward();

        // Assert
        Assert.Equal(1, simulation.CurrentStepIndex);
        Assert.Equal(SimulationStatus.Running, simulation.Status);

        // Check step completed event was raised
        Assert.Single(simulation.DomainEvents);
        var domainEvent = simulation.DomainEvents[0] as SimulationStepCompletedEvent;
        Assert.NotNull(domainEvent);
    }

    [Fact]
    public void StepForward_AtLastStep_ShouldCompleteSimulation()
    {
        // Arrange
        var simulation = new SimulationModel("int main() {}", "C");
        simulation.AddStep(new SimulationStep(1, 1, "int main()", OperationType.FunctionCall, "Main function entry"));
        simulation.Start();
        simulation.ClearDomainEvents(); // Clear start event

        // Act
        simulation.StepForward();

        // Assert
        Assert.Equal(SimulationStatus.Completed, simulation.Status);
        Assert.NotNull(simulation.CompletedAt);

        // Check simulation completed event was raised
        Assert.Single(simulation.DomainEvents);
        var domainEvent = simulation.DomainEvents[0] as SimulationCompletedEvent;
        Assert.NotNull(domainEvent);
    }

    [Fact]
    public void StepBackward_WithValidState_ShouldGoToPreviousStep()
    {
        // Arrange
        var simulation = new SimulationModel("int main() {}", "C");
        simulation.AddSteps(new[]
        {
            new SimulationStep(1, 1, "int main()", OperationType.FunctionCall, "Main function entry"),
            new SimulationStep(2, 2, "return 0;", OperationType.FunctionReturn, "Return")
        });

        simulation.Start();
        simulation.StepForward(); // Move to step 1

        // Act
        simulation.StepBackward();

        // Assert
        Assert.Equal(0, simulation.CurrentStepIndex);
        Assert.Equal(SimulationStatus.Running, simulation.Status);
    }

    [Fact]
    public void StepBackward_AtFirstStep_ShouldThrowInvalidOperationException()
    {
        // Arrange
        var simulation = new SimulationModel("int main() {}", "C");
        simulation.AddStep(new SimulationStep(1, 1, "int main()", OperationType.FunctionCall, "Main function entry"));
        simulation.Start(); // At step 0

        // Act & Assert
        var action = () => simulation.StepBackward();
        var exception = Assert.Throws<InvalidOperationException>(action);
        Assert.Contains("Already at the first step", exception.Message);
    }

    [Fact]
    public void Pause_WhenRunning_ShouldPauseSimulation()
    {
        // Arrange
        var simulation = new SimulationModel("int main() {}", "C");
        simulation.AddStep(new SimulationStep(1, 1, "int main()", OperationType.FunctionCall, "Main function entry"));
        simulation.Start();

        // Act
        simulation.Pause();

        // Assert
        Assert.Equal(SimulationStatus.Paused, simulation.Status);
    }

    [Fact]
    public void Resume_WhenPaused_ShouldResumeSimulation()
    {
        // Arrange
        var simulation = new SimulationModel("int main() {}", "C");
        simulation.AddStep(new SimulationStep(1, 1, "int main()", OperationType.FunctionCall, "Main function entry"));
        simulation.Start();
        simulation.Pause();

        // Act
        simulation.Resume();

        // Assert
        Assert.Equal(SimulationStatus.Running, simulation.Status);
    }

    [Fact]
    public void SetError_WithErrorMessage_ShouldSetErrorStatus()
    {
        var simulation = new SimulationModel("int main() {}", "C");
        simulation.AddStep(new SimulationStep(1, 1, "int main()", OperationType.FunctionCall, "Main function entry"));
        simulation.Start();
        simulation.ClearDomainEvents(); // Clear start event
        const string errorMessage = "Compilation error";

        // Act
        simulation.SetError(errorMessage);

        // Assert
        Assert.Equal(SimulationStatus.Error, simulation.Status);
        Assert.Equal(errorMessage, simulation.ErrorMessage);
        Assert.NotNull(simulation.CompletedAt);

        // Check domain event was raised
        Assert.Single(simulation.DomainEvents);
        var domainEvent = simulation.DomainEvents[0] as SimulationCompletedEvent;
        Assert.NotNull(domainEvent);
        Assert.Equal(SimulationStatus.Error, domainEvent.FinalStatus);
        Assert.Equal(errorMessage, domainEvent.ErrorMessage);
    }

    [Fact]
    public void Reset_ShouldResetSimulationState()
    {
        // Arrange
        var simulation = new SimulationModel("int main() {}", "C");
        simulation.AddStep(new SimulationStep(1, 1, "int main()", OperationType.FunctionCall, "Main function entry"));
        simulation.Start();
        simulation.SetError("Some error");

        // Act
        simulation.Reset();

        // Assert
        Assert.Equal(SimulationStatus.Reset, simulation.Status);
        Assert.Equal(-1, simulation.CurrentStepIndex);
        Assert.Null(simulation.StartedAt);
        Assert.Null(simulation.CompletedAt);
        Assert.Null(simulation.ErrorMessage);
    }

    [Theory]
    [InlineData(-1)]
    [InlineData(2)] // When we only have to step (index 0)
    public void GoToStep_WithInvalidIndex_ShouldThrowArgumentOutOfRangeException(int index)
    {
        // Arrange
        var simulation = new SimulationModel("int main() {}", "C");
        simulation.AddStep(new SimulationStep(1, 1, "int main()", OperationType.FunctionCall, "Main function entry"));
        simulation.Start();

        // Act & Assert
        var action = () => simulation.GoToStep(index);
        var exception = Assert.Throws<ArgumentOutOfRangeException>(action);
    }

    [Fact]
    public void GoToStep_WithValidIndex_ShouldJumpToStep()
    {
        // Arrange
        var simulation = new SimulationModel("int main() {}", "C");
        simulation.AddSteps(new[]
        {
            new SimulationStep(1, 1, "int main()", OperationType.FunctionCall, "Main function entry"),
            new SimulationStep(2, 2, "return 0;", OperationType.FunctionReturn, "Return")
        });
        simulation.Start();
        simulation.ClearDomainEvents();

        // Act
        simulation.GoToStep(1);

        // Assert
        Assert.Equal(1, simulation.CurrentStepIndex);
        Assert.Equal(SimulationStatus.Running, simulation.Status);
        // Should raise step completed event.
        Assert.Single(simulation.DomainEvents);
        Assert.IsType<SimulationStepCompletedEvent>(simulation.DomainEvents[0]);
    }
}
