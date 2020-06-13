using MediatR;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace PetDoctor.API.Application.Commands
{
    public class CancelAppointmentHandler : IRequestHandler<CancelAppointment, Unit>
    {
        public Task<Unit> Handle(CancelAppointment request, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }
    }
}
