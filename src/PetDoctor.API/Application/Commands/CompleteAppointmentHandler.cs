using MediatR;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace PetDoctor.API.Application.Commands
{
    public class CompleteAppointmentHandler : IRequestHandler<CompleteAppointment, Unit>
    {
        public Task<Unit> Handle(CompleteAppointment request, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }
    }
}
