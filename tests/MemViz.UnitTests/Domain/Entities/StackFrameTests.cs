using MemViz.Domain.Entities;
using MemViz.Domain.Enums;
using MemViz.Domain.ValueObjects;

namespace MemViz.UnitTests.Domain.Entities;

public class StackFrameTests
{
    [Fact]
    public void Constructor_WithValidParameters_ShouldCreateStackFrame()
    {
        // Arrange
        const string functionName = "main";
        var baseAddress = MemoryAddress.Generate(isStack: true);
        const int lineNumber = 5;

        // Act
        var stackFrame = new StackFrame(functionName, baseAddress, lineNumber);

        // Assert
        Assert.NotNull(stackFrame);
        Assert.Equal(functionName, stackFrame.FunctionName);
        Assert.Equal(baseAddress, stackFrame.BaseAddress);
        Assert.Equal(lineNumber, stackFrame.LineNumber);
        Assert.Equal(0, stackFrame.Size);
        Assert.Empty(stackFrame.Variables);
    }

    [Theory]
    [InlineData("")]
    [InlineData("   ")]
    [InlineData(null)]
    public void Constructor_WithInvalidFunctionName_ShouldThrowArgumentException(string functionName)
    {
        // Arrange
        var baseAddress = MemoryAddress.Generate(true);

        // Act & Assert
        var action = () => new StackFrame(functionName, baseAddress);
        Assert.Throws<ArgumentException>(action);
    }

    [Fact]
    public void AddVariable_WithNewVariable_ShouldAddToCollection()
    {
        // Arrange
        var stackFrame = new StackFrame("main", MemoryAddress.Generate(true));
        var variable = new Variable("x", ValueTypeKind.Integer, MemoryAddress.Generate(true), 4, "42");

        // Act
        stackFrame.AddVariable(variable);

        // Assert
        Assert.Single(stackFrame.Variables);
        Assert.Contains(variable, stackFrame.Variables);
        Assert.Equal(4, stackFrame.Size); // Size should be updated
    }

    [Fact]
    public void AddVariable_WithDuplicateName_ShouldThrowInvalidOperationException()
    {
        // Arrange
        var stackFrame = new StackFrame("main", MemoryAddress.Generate(true));
        var variable1 = new Variable("x", ValueTypeKind.Integer, MemoryAddress.Generate(true), 4);
        var variable2 = new Variable("x", ValueTypeKind.Float, MemoryAddress.Generate(true), 8);
        stackFrame.AddVariable(variable1);

        // Act & Assert
        var action = () => stackFrame.AddVariable(variable2);
        var exception = Assert.Throws<InvalidOperationException>(action);
        Assert.Contains("already exists", exception.Message);
    }

    [Fact]
    public void RemoveVariable_WithExistingVariable_ShouldRemoveFromCollection()
    {
        // Arrange
        var stackFrame = new StackFrame("main", MemoryAddress.Generate(true));
        var variable = new Variable("x", ValueTypeKind.Integer, MemoryAddress.Generate(true), 4);
        stackFrame.AddVariable(variable);

        // Act
        stackFrame.RemoveVariable("x");

        // Assert
        Assert.Empty(stackFrame.Variables);
        Assert.Equal(0, stackFrame.Size); // Size should be updated
    }

    [Fact]
    public void RemoveVariable_WithNonExistentVariable_ShouldNotThrow()
    {
        // Arrange
        var stackFrame = new StackFrame("main", MemoryAddress.Generate(true));

        // Act & Assert (should not throw)
        stackFrame.RemoveVariable("nonExistentVar");
        Assert.Empty(stackFrame.Variables);
    }

    [Fact]
    public void GetVariable_WithExistingName_ShouldReturnVariable()
    {
        // Arrange
        var stackFrame = new StackFrame("main", MemoryAddress.Generate(true));
        var variable = new Variable("x", ValueTypeKind.Integer, MemoryAddress.Generate(true), 4);
        stackFrame.AddVariable(variable);

        // Act
        var retrievedVar = stackFrame.GetVariable("x");

        // Assert
        Assert.NotNull(retrievedVar);
        Assert.Equal(variable, retrievedVar);
    }

    [Fact]
    public void GetVariable_WithNonExistentName_ShouldReturnNull()
    {
        // Arrange
        var stackFrame = new StackFrame("main", MemoryAddress.Generate(true));

        // Act
        var retrievedVar = stackFrame.GetVariable("nonExistentVar");

        // Assert
        Assert.Null(retrievedVar);
    }

    [Fact]
    public void UpdateLineNumber_ShouldUpdateLineNumber()
    {
        // Arrange
        var stackFrame = new StackFrame("main", MemoryAddress.Generate(true), 5);

        // Act
        stackFrame.UpdateLineNumber(5);

        // Assert
        Assert.Equal(5, stackFrame.LineNumber);
    }
}