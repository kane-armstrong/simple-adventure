using Microsoft.Extensions.DependencyInjection;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace PetDoctor.Infrastructure.Cqrs;

public class EventDispatcher : IEventDispatcher
{
    private readonly IServiceProvider _serviceProvider;

    public EventDispatcher(IServiceProvider serviceProvider)
    {
        _serviceProvider = serviceProvider;
    }

    public async Task Dispatch<TEvent>(TEvent @event, CancellationToken cancellationToken = default)
    {
        // TODO  gross, do this properly
        using var scope = _serviceProvider.CreateScope();
        var handlerType = typeof(IEventHandler<>).MakeGenericType(@event.GetType());
        dynamic handler = scope.ServiceProvider.GetRequiredService(handlerType);
        await handler.Handle((dynamic)@event, cancellationToken);
    }
}