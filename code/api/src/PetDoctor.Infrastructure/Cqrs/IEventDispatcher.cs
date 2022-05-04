namespace PetDoctor.Infrastructure.Cqrs;

public interface IEventDispatcher
{
    Task Dispatch<TEvent>(TEvent @event, CancellationToken cancellationToken = default) where TEvent : class;
}