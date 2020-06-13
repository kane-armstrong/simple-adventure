using MediatR;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace PetDoctor.API.Application.Commands
{
    public class CheckinToAppointmentHandler : IRequestHandler<CheckinToAppointment, Unit>
    {
        public Task<Unit> Handle(CheckinToAppointment request, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }
    }
}
