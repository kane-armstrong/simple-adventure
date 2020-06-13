using MediatR;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace PetDoctor.API.Application.Commands
{
    public class CreateAppointmentHandler : IRequestHandler<CreateAppointment, CommandResult>
    {
        public Task<CommandResult> Handle(CreateAppointment request, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }
    }
}