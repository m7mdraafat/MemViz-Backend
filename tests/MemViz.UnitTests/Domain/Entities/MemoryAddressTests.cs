using MemViz.Domain.ValueObjects;

namespace MemViz.UnitTests.Domain.Entities;

public class MemoryAddressTests
{
    [Fact]
    public void FromHex_WithValidHexString_ShouldCreateMemoryAddress()
    {
        // Arrange
        const string hexAddress = "0X7FFF0000";

        // Act
        var address = MemoryAddress.FromHex(hexAddress);

        // Assert
        Assert.NotNull(address);
        Assert.Equal(hexAddress, address.ToString());
    }

    [Theory]
    [InlineData("7FFF0000")]  // Missing '0x' prefix
    [InlineData("")]           // Empty string
    [InlineData(null)]         // Null string
    [InlineData("   ")]         // Only whitespace
    public void FromHex_WithInvalidString_ShouldThrowArgumentException(string hexAddress)
    {
        // Act & Assert
        var action = () => MemoryAddress.FromHex(hexAddress);
        Assert.Throws<ArgumentException>(action);
    }

    [Fact]
    public void FromHex_WithValidLong_ShouldCreateMemoryAddress()
    {
        // Arrange
        const long longAddress = 0x7FFF0000L;

        // Act
        var address = MemoryAddress.FromLong(longAddress);

        // Assert
        Assert.NotNull(address);
        Assert.Equal("0x7FFF0000", address.ToString());
    }

    [Fact]
    public void Generate_ForStack_ShouldCreateStackAddress()
    {
        // Act
        var address = MemoryAddress.Generate(isStack: true);

        // Assert
        Assert.NotNull(address);
        Assert.StartsWith("0x7FFF", address.ToString());
    }

    [Fact]
    public void Generate_ForHeap_ShouldCreateHeapAddress()
    {
        // Act
        var address = MemoryAddress.Generate(isStack: false);

        // Assert
        Assert.NotNull(address);
        Assert.StartsWith("0x", address.ToString());

        Assert.False(address.ToString().StartsWith("0x7FFF"));
    }

    [Fact]
    public void RecordEquality_ShouldWorkCorrectly()
    {
        // Arrange
        var address1 = MemoryAddress.FromHex("0x1000");
        var address2 = MemoryAddress.FromHex("0x1000");
        var address3 = MemoryAddress.FromHex("0x2000");

        // Assert
        Assert.Equal(address1, address2);
        Assert.NotEqual(address1, address3);
        Assert.True(address1 == address2);
        Assert.True(address1 != address3);
        Assert.False(address1 == address3);
        Assert.False(address1 != address2);
    }
}