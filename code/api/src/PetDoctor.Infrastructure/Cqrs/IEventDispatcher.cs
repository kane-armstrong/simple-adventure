using PetDoctor.Domain;
using System.Threading;
using System.Threading.Tasks;

namespace PetDoctor.Infrastructure.Cqrs;

public interface IEventDispatcher
{
    Task Dispatch<TEvent>(TEvent @event, CancellationToken cancellationToken = default);
}