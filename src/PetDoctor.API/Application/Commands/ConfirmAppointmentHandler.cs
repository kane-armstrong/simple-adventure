using MediatR;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace PetDoctor.API.Application.Commands
{
    public class ConfirmAppointmentHandler : IRequestHandler<ConfirmAppointment, CommandContext>
    {
        public Task<CommandContext> Handle(ConfirmAppointment request, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }
    }
}
