using MemViz.Domain.Common;
using MemViz.Domain.ValueObjects;

namespace MemViz.Domain.Entities;

/// <summary>
/// Represents a stack frame for a function call
/// </summary>
public class StackFrame : Entity
{
    public string FunctionName { get; private set; }
    public MemoryAddress BaseAddress { get; private set; }
    public int Size { get; private set; }
    public int LineNumber { get; private set; }
    public DateTime CreatedAt { get; private set; }

    private readonly List<Variable> _variables;
    public IReadOnlyList<Variable> Variables => _variables.AsReadOnly();

    private StackFrame() : base()
    {
        _variables = new List<Variable>();
    }

    public StackFrame(
        string functionName,
        MemoryAddress baseAddress,
        int lineNumber = 0)
    {
        if (string.IsNullOrWhiteSpace(functionName))
            throw new ArgumentException("Function name cannot be null or empty", nameof(functionName));
        
        FunctionName = functionName;
        BaseAddress = baseAddress;
        LineNumber = lineNumber;
        Size = 0;
        CreatedAt = DateTime.UtcNow;
        _variables = new List<Variable>();
    }
    
    public void AddVariable(Variable variable)
    {
        if (_variables.Any(v => v.Name == variable.Name))
            throw new InvalidOperationException($"Variable '{variable.Name}' already exists in this stack frame");
        
        _variables.Add(variable);
        Size += variable.Size;
    }
    
    public void RemoveVariable(string variableName)
    {
        var variable = _variables.FirstOrDefault(v => v.Name == variableName);
        if (variable != null)
        {
            _variables.Remove(variable);
            Size -= variable.Size;
        }
    }
    
    public Variable? GetVariable(string name)
    {
        return _variables.FirstOrDefault(v => v.Name == name);
    }
    
    public void UpdateLineNumber(int lineNumber)
    {
        LineNumber = lineNumber;
    }
}