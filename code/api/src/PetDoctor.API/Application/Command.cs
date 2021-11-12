using MediatR;

namespace PetDoctor.API.Application;

public abstract record Command : IRequest<CommandResult>
{
}