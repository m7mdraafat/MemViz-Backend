namespace MemViz.Domain.Enums;

/// <summary>
/// Represents the type of memory operation performed
/// </summary>
public enum OperationType
{
    /// <summary>
    /// Variable declaration
    /// </summary>
    Declaration = 0,
    
    /// <summary>
    /// Variable assignment
    /// </summary>
    Assignment = 1,
    
    /// <summary>
    /// Memory allocation (malloc, new, etc.)
    /// </summary>
    Allocation = 2,
    
    /// <summary>
    /// Memory deallocation (free, delete, etc.)
    /// </summary>
    Deallocation = 3,
    
    /// <summary>
    /// Function call - push new stack frame
    /// </summary>
    FunctionCall = 4,
    
    /// <summary>
    /// Function return - pop stack frame
    /// </summary>
    FunctionReturn = 5,
    
    /// <summary>
    /// Pointer/Reference assignment
    /// </summary>
    PointerAssignment = 6,
    
    /// <summary>
    /// Array access
    /// </summary>
    ArrayAccess = 7,
    
    /// <summary>
    /// Struct/Object member access
    /// </summary>
    MemberAccess = 8
}