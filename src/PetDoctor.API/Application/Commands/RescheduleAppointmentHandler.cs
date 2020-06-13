using MediatR;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace PetDoctor.API.Application.Commands
{
    public class RescheduleAppointmentHandler : IRequestHandler<RescheduleAppointment, Unit>
    {
        public Task<Unit> Handle(RescheduleAppointment request, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }
    }
}
