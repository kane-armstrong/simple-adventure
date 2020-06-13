using MediatR;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace PetDoctor.API.Application.Commands
{
    public class RejectAppointmentHandler : IRequestHandler<RejectAppointment, CommandResult>
    {
        public Task<CommandResult> Handle(RejectAppointment request, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }
    }
}
