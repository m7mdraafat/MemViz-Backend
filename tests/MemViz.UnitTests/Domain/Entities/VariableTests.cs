using MemViz.Domain.Entities;
using MemViz.Domain.Enums;
using MemViz.Domain.ValueObjects;

namespace MemViz.UnitTests.Domain.Entities;

public class VariableTests
{
    [Fact]
    public void Constructor_WithValidParameters_ShouldCreateVariable()
    {
        // Arrange
        const string name = "x";
        const ValueTypeKind type = ValueTypeKind.Integer;
        var address = MemoryAddress.Generate(isStack: true);
        const int size = 4;
        const string value = "42";

        // Act
        var variable = new Variable(name, type, address, size, value);

        // Assert
        Assert.NotNull(variable);
        Assert.Equal(name, variable.Name);
        Assert.Equal(type, variable.Type);
        Assert.Equal(address, variable.Address);
        Assert.Equal(size, variable.Size);
        Assert.Equal(value, variable.Value);
    }

    [Fact]
    public void Constructor_WithoutInitialValue_ShouldCreateUninitializedVariable()
    {
        // Arrange & Act
        var variable = new Variable("y", ValueTypeKind.Float, MemoryAddress.Generate(true), 4);

        // Assert
        Assert.NotNull(variable);
        Assert.Equal("y", variable.Name);
        Assert.Equal(ValueTypeKind.Float, variable.Type);
        Assert.Equal(4, variable.Size);
        Assert.Null(variable.Value);
    }

    [Theory]
    [InlineData("")]
    [InlineData("   ")]
    [InlineData(null)]
    public void Constructor_WithInvalidName_ShouldThrowArgumentException(string name)
    {
        // Act & Assert
        var action = () => new Variable(name, ValueTypeKind.Integer, MemoryAddress.Generate(true), 4);
        Assert.Throws<ArgumentException>(action);
    }

    [Theory]
    [InlineData(0)]
    [InlineData(-1)]
    public void Constructor_WithInvalidSize_ShouldThrowArgumentException(int size)
    {
        // Act & Assert
        var action = () => new Variable("z", ValueTypeKind.Integer, MemoryAddress.Generate(true), size);
        Assert.Throws<ArgumentException>(action);
    }

    [Fact]
    public void UpdateValue_ShouldUpdateValueAndMarkInitialized()
    {
        // Arrange
        var variable = new Variable("a", ValueTypeKind.Integer, MemoryAddress.Generate(true), 4);
        const string newValue = "100";

        // Act
        variable.UpdateValue(newValue);

        // Assert
        Assert.Equal(newValue, variable.Value);
        Assert.True(variable.IsInitialized);
    }

    [Fact]
    public void SetPointerTarget_WithPointerType_ShouldSetTarget()
    {
        // Arrange
        var pointerVariable = new Variable("ptr", ValueTypeKind.Pointer, MemoryAddress.Generate(true), 8);
        var targetAddress = MemoryAddress.Generate(isStack: false);

        // Act
        pointerVariable.SetPointerTarget(targetAddress);

        // Assert
        Assert.Equal(targetAddress, pointerVariable.PointsTo);
        Assert.Equal(pointerVariable.Value, targetAddress.Value);
        Assert.True(pointerVariable.IsInitialized);
    }

    [Fact]
    public void SetPointerTarget_WithNonPointerType_ShouldThrowInvalidOperationException()
    {
        // Arrange
        var variable = new Variable("x", ValueTypeKind.Integer, MemoryAddress.Generate(true), 4);
        var targetAddress = MemoryAddress.Generate();

        // Act & Assert
        var action = () => variable.SetPointerTarget(targetAddress);
        Assert.Throws<InvalidOperationException>(action)
        .Message.Contains("Only pointer variables can have a target address");
    }

    [Fact]
    public void ClearPointerTarget_ShouldSetToNullptr()
    {
        // Arrange
        var variable = new Variable("ptr", ValueTypeKind.Pointer, MemoryAddress.Generate(true), 8);
        variable.SetPointerTarget(MemoryAddress.Generate(isStack: false));

        // Act
        variable.ClearPointerTarget();

        // Assert
        Assert.Null(variable.PointsTo);
        Assert.False(variable.IsInitialized);
        Assert.Equal("nullptr", variable.Value);
    }
}