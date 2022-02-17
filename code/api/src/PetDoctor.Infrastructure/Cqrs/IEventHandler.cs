using System.Threading;
using System.Threading.Tasks;

namespace PetDoctor.Infrastructure.Cqrs;

public interface IEventHandler<in TEvent>
{
    Task Handle(TEvent @event, CancellationToken cancellationToken);
}