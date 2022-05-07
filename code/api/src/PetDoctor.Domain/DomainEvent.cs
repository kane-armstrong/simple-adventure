namespace PetDoctor.Domain;

public abstract record DomainEvent
{
    public Guid Id { get; } = Guid.NewGuid();
}