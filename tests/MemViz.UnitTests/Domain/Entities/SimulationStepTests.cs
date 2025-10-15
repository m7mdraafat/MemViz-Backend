
using MemViz.Domain.Entities;
using MemViz.Domain.Enums;
using MemViz.Domain.ValueObjects;

namespace MemViz.UnitTests.Domain.Entities;
public class SimulationStepTests
{
    [Fact]
    public void Constructor_WithValidParameters_ShouldCreateStep()
    {
        // Arrange
        var stepNumber = 1;
        var lineNumber = 5;
        const string codeLine = "int x = 42";
        const OperationType operation = OperationType.Assignment;
        const string description = "Assigned value 42 to variable x";

        // Act
        var step = new SimulationStep(stepNumber, lineNumber, codeLine, operation, description);

        // Assert
        Assert.Equal(stepNumber, step.StepNumber);
        Assert.Equal(lineNumber, step.LineNumber);
        Assert.Equal(codeLine, step.CodeLine);
        Assert.Equal(operation, step.Operation);
        Assert.Equal(description, step.Description);
        Assert.Empty(step.StackFrames);
        Assert.Empty(step.HeapObjects);
        Assert.Empty(step.Pointers);
    }

    [Theory]
    [InlineData(-1, 1, "code", OperationType.Declaration, "desc")]
    [InlineData(1, -1, "code", OperationType.Declaration, "desc")]
    [InlineData(1, 1, "", OperationType.Declaration, "desc")]
    [InlineData(1, 1, "   ", OperationType.Declaration, "desc")]
    [InlineData(1, 1, null, OperationType.Declaration, "desc")]
    public void Constructor_WithInvalidParameters_ShouldThrowArgumentException(
        int stepNumber, int lineNumber, string codeLine, OperationType operation, string description)
    {
        // Act & Assert
        var action = () => new SimulationStep(stepNumber, lineNumber, codeLine, operation, description);
        Assert.Throws<ArgumentException>(action);
    }

    [Fact]
    public void Constructor_WithNullDescription_ShouldUseEmptyString()
    {
        // Arrange
        var step = new SimulationStep(1, 1, "code", OperationType.Declaration, null);

        // Assert
        Assert.Equal(string.Empty, step.Description);
    }

    [Fact]
    public void AddStackFrame_ShouldAddToCollection()
    {
        // Arrange
        var step = new SimulationStep(1, 1, "code", OperationType.Declaration, "desc");
        var stackFrame = new StackFrame("main", MemoryAddress.Generate(true));

        // Act
        step.AddStackFrame(stackFrame);

        // Assert
        Assert.Single(step.StackFrames);
        Assert.Contains(stackFrame, step.StackFrames);
    }

    [Fact]
    public void SetStackFrames_ShouldReplaceAllFrames()
    {
        // Arrange
        var step = new SimulationStep(1, 1, "code", OperationType.Declaration, "desc");
        var frame1 = new StackFrame("main", MemoryAddress.Generate(true));
        var frame2 = new StackFrame("func", MemoryAddress.Generate(true));
        step.AddStackFrame(frame1);

        var newFrames = new List<StackFrame> { frame2 };

        // Act
        step.SetStackFrames(newFrames);

        // Assert
        Assert.Single(step.StackFrames);
        Assert.Contains(frame2, step.StackFrames);
        Assert.DoesNotContain(frame1, step.StackFrames);
    }

    [Fact]
    public void AddHeapObject_ShouldAddToCollection()
    {
        // Arrange
        var step = new SimulationStep(1, 1, "code", OperationType.Declaration, "desc");
        var heapObject = new HeapObject("obj", ValueTypeKind.Struct, MemoryAddress.Generate(), 8);

        // Act
        step.AddHeapObject(heapObject);

        // Assert
        Assert.Single(step.HeapObjects);
        Assert.Contains(heapObject, step.HeapObjects);
    }

    [Fact]
    public void AddPointer_ShouldAddToCollection()
    {
        // Arrange
        var step = new SimulationStep(1, 1, "code", OperationType.Declaration, "desc");
        var pointer = new Pointer("ptr", MemoryAddress.Generate(true), MemoryAddress.Generate());

        // Act
        step.AddPointer(pointer);

        // Assert
        Assert.Single(step.Pointers);
        Assert.Contains(pointer, step.Pointers);
    }
}
