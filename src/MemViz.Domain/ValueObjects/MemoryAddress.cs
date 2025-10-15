namespace MemViz.Domain.ValueObjects;

/// <summary>
/// Value object representing a memory address
/// </summary>
public sealed record MemoryAddress
{
    public string Value { get; init; }

    private MemoryAddress(string value)
    {
        if (string.IsNullOrWhiteSpace(value))
            throw new ArgumentException("Memory address cannot be null or empty", nameof(value));

        Value = value;
    }

    /// <summary>
    /// Creates a new memory address from a hexadecimal string
    /// </summary>
    public static MemoryAddress FromHex(string hexAddress)
    {
        if (string.IsNullOrWhiteSpace(hexAddress) || !hexAddress.StartsWith("0x", StringComparison.OrdinalIgnoreCase))
            throw new ArgumentException("Memory address must start with '0x'", nameof(hexAddress));

        return new MemoryAddress(hexAddress.ToUpper());
    }
    
    /// <summary>
    /// Creates a new memory address from a long value
    /// </summary>
    public static MemoryAddress FromLong(long address)
    {
        return new MemoryAddress($"0x{address:X}");
    }
    
    /// <summary>
    /// Generates a random memory address for simulation purposes
    /// </summary>
    public static MemoryAddress Generate(bool isStack = false)
    {
        var random = new Random();

        long baseAddress = isStack 
            ? 0x7FFF_0000_0000 + random.Next(0, 0x7FFF_FFFF)
            : 0x0000_0000_1000 + random.Next(0, 0x7FFF_FFFF);
            
        return FromLong(baseAddress);
    }
    
    /// <summary>
    /// Creates a null/invalid memory address
    /// </summary>
    public static MemoryAddress Null => new("0x0");
    
    public override string ToString() => Value;
}