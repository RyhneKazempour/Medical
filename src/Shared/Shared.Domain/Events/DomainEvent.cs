namespace MyApp.Shared.Domain.Events;

public abstract record DomainEvent(Guid EventId, DateTimeOffset OccurredAt)
{
    protected DomainEvent() : this(Guid.NewGuid(), DateTimeOffset.UtcNow) { }
}
