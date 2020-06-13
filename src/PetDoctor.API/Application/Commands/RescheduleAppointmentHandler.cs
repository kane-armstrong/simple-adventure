using MediatR;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace PetDoctor.API.Application.Commands
{
    public class RescheduleAppointmentHandler : IRequestHandler<RescheduleAppointment, CommandResult>
    {
        public Task<CommandResult> Handle(RescheduleAppointment request, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }
    }
}
