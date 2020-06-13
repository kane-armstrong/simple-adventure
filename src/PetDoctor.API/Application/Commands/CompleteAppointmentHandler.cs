using MediatR;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace PetDoctor.API.Application.Commands
{
    public class CompleteAppointmentHandler : IRequestHandler<CompleteAppointment, CommandResult>
    {
        public Task<CommandResult> Handle(CompleteAppointment request, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }
    }
}
