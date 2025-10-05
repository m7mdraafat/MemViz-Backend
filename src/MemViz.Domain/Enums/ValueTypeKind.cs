namespace MemViz.Domain.Enums;

/// <summary>
/// Represents the kind/type of value stored in memory
/// </summary>
public enum ValueTypeKind
{
    /// <summary>
    /// Integer type (int, short, long, etc.)
    /// </summary>
    Integer = 0,
    
    /// <summary>
    /// Floating point type (float, double)
    /// </summary>
    Float = 1,
    
    /// <summary>
    /// Character type
    /// </summary>
    Character = 2,
    
    /// <summary>
    /// Boolean type
    /// </summary>
    Boolean = 3,
    
    /// <summary>
    /// Pointer/Reference type
    /// </summary>
    Pointer = 4,
    
    /// <summary>
    /// Array type
    /// </summary>
    Array = 5,
    
    /// <summary>
    /// Struct/Object type
    /// </summary>
    Struct = 6,
    
    /// <summary>
    /// String type
    /// </summary>
    String = 7,
    
    /// <summary>
    /// Void type (no value)
    /// </summary>
    Void = 8,
    
    /// <summary>
    /// Unknown or uninitialized type
    /// </summary>
    Unknown = 9
}