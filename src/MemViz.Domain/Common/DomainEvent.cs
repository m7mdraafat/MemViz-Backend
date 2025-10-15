namespace MemViz.Domain.Common;

/// <summary>
/// Base class for domain events.
/// </summary>
public abstract record DomainEvent
{
    public Guid Id { get; init; }
    public DateTime OccurredOn { get; init; }

    protected DomainEvent()
    {
        Id = Guid.NewGuid();
        OccurredOn = DateTime.UtcNow;
    }
}