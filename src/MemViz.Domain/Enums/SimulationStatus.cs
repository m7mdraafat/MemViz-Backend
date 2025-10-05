namespace MemViz.Domain.Enums;

/// <summary>
/// Represents the current status of a simulation
/// </summary>
public enum SimulationStatus
{
    /// <summary>
    /// Simulation is ready to start
    /// </summary>
    Ready = 0,
    
    /// <summary>
    /// Simulation is currently running
    /// </summary>
    Running = 1,
    
    /// <summary>
    /// Simulation is paused
    /// </summary>
    Paused = 2,
    
    /// <summary>
    /// Simulation has completed successfully
    /// </summary>
    Completed = 3,
    
    /// <summary>
    /// Simulation encountered an error
    /// </summary>
    Error = 4,
    
    /// <summary>
    /// Simulation was reset
    /// </summary>
    Reset = 5
}