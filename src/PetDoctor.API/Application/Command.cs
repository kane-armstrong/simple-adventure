using MediatR;

namespace PetDoctor.API.Application
{
    public class Command : IRequest<CommandContext>
    {
        internal CommandContext Context { get; set; }
    }
}
