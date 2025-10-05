namespace MemViz.Domain.Enums;

/// <summary>
/// Represents the type of memory segment where data is stored
/// </summary>
public enum MemorySegmentType
{
    /// <summary>
    /// Stack memory - LIFO, automatic allocation/deallocation
    /// </summary>
    Stack = 0,
    
    /// <summary>
    /// Heap memory - dynamic allocation, manual management
    /// </summary>
    Heap = 1,
    
    /// <summary>
    /// Static/Global memory - program lifetime
    /// </summary>
    Static = 2,
    
    /// <summary>
    /// Code/Text segment - read-only, contains program instructions
    /// </summary>
    Code = 3
}