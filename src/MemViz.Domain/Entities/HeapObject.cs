using MemViz.Domain.Common;
using MemViz.Domain.Enums;
using MemViz.Domain.ValueObjects;

namespace MemViz.Domain.Entities;

/// <summary>
/// Represents an object allocated on the heap.
/// </summary>
public class HeapObject : Entity
{
    public string Name { get; private set; }
    public ValueTypeKind Type { get; private set; }
    public MemoryAddress Address { get; private set; }
    public int Size { get; private set; }
    public bool IsAllocated { get; private set; }
    public DateTime AllocatedAt { get; private set; }
    public DateTime? DeallocatedAt { get; private set; }

    private readonly List<Variable> _fields;
    public IReadOnlyList<Variable> Fields => _fields.AsReadOnly();

    private HeapObject() : base()
    {
        _fields = new List<Variable>();
    }

    public HeapObject(
        string name,
        ValueTypeKind type,
        MemoryAddress address,
        int size)
    {
        if (string.IsNullOrWhiteSpace(name))
            throw new ArgumentException("Heap object name cannot be null or empty", nameof(name));

        if (size <= 0)
            throw new ArgumentException("Heap object size must be positive", nameof(size));

        Name = name;
        Type = type;
        Address = address;
        Size = size;
        IsAllocated = true;
        AllocatedAt = DateTime.UtcNow;
        DeallocatedAt = null;
        _fields = new List<Variable>();
    }

    public void AddField(Variable field)
    {
        if (!IsAllocated)
            throw new InvalidOperationException("Cannot add fields to a deallocated heap object.");

        if (_fields.Any(f => f.Name == field.Name))
            throw new InvalidOperationException($"Field '{field.Name}' already exists in this heap object.");

        _fields.Add(field);
    }

    public Variable? GetField(string name)
    {
        return _fields.FirstOrDefault(f => f.Name == name);
    }

    public void Deallocate()
    {
        if (!IsAllocated)
            throw new InvalidOperationException("Heap object is already deallocated.");

        IsAllocated = false;
        DeallocatedAt = DateTime.UtcNow;
    }
}