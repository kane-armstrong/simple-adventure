using MediatR;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace PetDoctor.API.Application.Commands
{
    public class ConfirmAppointmentHandler : IRequestHandler<ConfirmAppointment, CommandResult>
    {
        public Task<CommandResult> Handle(ConfirmAppointment request, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }
    }
}
