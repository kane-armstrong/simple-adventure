namespace PetDoctor.Domain;

public abstract class EventSourcedEntity : IEventSourcedEntity
{
    public Guid Id { get; protected set; }
    public IReadOnlyList<DomainEvent> PendingEvents => _events.AsReadOnly();

    private readonly List<DomainEvent> _events = new List<DomainEvent>();
    public void AppendEvent(DomainEvent @event)
    {
        _events.Add(@event);
    }
}