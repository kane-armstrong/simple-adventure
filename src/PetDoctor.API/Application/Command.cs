using MediatR;

namespace PetDoctor.API.Application
{
    public class Command : IRequest<CommandResult>
    {
        internal CommandContext Context { get; set; }
    }
}
