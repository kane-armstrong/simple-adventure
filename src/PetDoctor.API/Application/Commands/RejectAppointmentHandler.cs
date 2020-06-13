using MediatR;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace PetDoctor.API.Application.Commands
{
    public class RejectAppointmentHandler : IRequestHandler<RejectAppointment, Unit>
    {
        public Task<Unit> Handle(RejectAppointment request, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }
    }
}
