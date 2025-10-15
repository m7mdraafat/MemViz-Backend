using MemViz.Domain.Common;
using MemViz.Domain.Enums;
using MemViz.Domain.ValueObjects;

namespace MemViz.Domain.Entities;

/// <summary>
/// Represents a variable in memory (Stack or Heap)
/// </summary>
public class Variable : Entity
{
    public string Name { get; private set; }
    public ValueTypeKind Type { get; private set; }
    public string? Value { get; private set; }
    public MemoryAddress Address { get; private set; }
    public int Size { get; private set; }
    public bool IsInitialized { get; private set; }
    public MemoryAddress? PointsTo { get; private set; }

    private Variable() : base() { }

    public Variable(
        string name,
        ValueTypeKind type,
        MemoryAddress address,
        int size,
        string? initialValue = null)
    {
        if (string.IsNullOrWhiteSpace(name))
            throw new ArgumentException("Variable name cannot be null or empty", nameof(name));

        if (size <= 0)
            throw new ArgumentException("Variable size must be positive", nameof(size));

        Name = name;
        Type = type;
        Address = address;
        Size = size;
        Value = initialValue;
        IsInitialized = initialValue != null;
    }

    public void UpdateValue(string newValue)
    {
        Value = newValue;
        IsInitialized = true;
    }

    public void SetPointerTarget(MemoryAddress targetAddress)
    {
        if (Type != ValueTypeKind.Pointer)
            throw new InvalidOperationException("Only pointer variables can have a target address.");

        PointsTo = targetAddress;
        Value = targetAddress.Value;
        IsInitialized = true;
    }

    public void ClearPointerTarget()
    {
        PointsTo = null;
        Value = "nullptr";
    }
}