using MediatR;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace PetDoctor.API.Application.Commands
{
    public class CreateAppointmentHandler : IRequestHandler<CreateAppointment, CommandContext>
    {
        public Task<CommandContext> Handle(CreateAppointment request, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }
    }
}
