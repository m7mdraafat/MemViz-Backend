using MemViz.Domain.Common;
using MemViz.Domain.ValueObjects;

namespace MemViz.Domain.Entities;

/// <summary>
/// Represents a pointer connection between stack and heap or between memory locations
/// </summary>
public class Pointer : Entity
{
    public string Name { get; private set; }
    public MemoryAddress SourceAddress { get; private set; }
    public MemoryAddress TargetAddress { get; private set; }
    public string? Label { get; private set; }
    public bool IsValid { get; private set; }
    
    private Pointer() : base() { }
    
    public Pointer(
        string name,
        MemoryAddress sourceAddress,
        MemoryAddress targetAddress,
        string? label = null)
    {
        if (string.IsNullOrWhiteSpace(name))
            throw new ArgumentException("Pointer name cannot be null or empty", nameof(name));
        
        Name = name;
        SourceAddress = sourceAddress;
        TargetAddress = targetAddress;
        Label = label;
        IsValid = targetAddress != MemoryAddress.Null;
    }
    
    public void UpdateTarget(MemoryAddress newTargetAddress)
    {
        TargetAddress = newTargetAddress;
        IsValid = newTargetAddress != MemoryAddress.Null;
    }
    
    public void Invalidate()
    {
        IsValid = false;
        TargetAddress = MemoryAddress.Null;
    }
}