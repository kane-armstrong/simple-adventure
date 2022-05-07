namespace PetDoctor.Domain;

public interface IEventSourcedEntity
{
    IReadOnlyList<DomainEvent> PendingEvents { get; }
}