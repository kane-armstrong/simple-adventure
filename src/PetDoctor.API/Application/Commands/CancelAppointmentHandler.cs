using MediatR;
using PetDoctor.Domain.Aggregates.Appointments;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace PetDoctor.API.Application.Commands
{
    public class CancelAppointmentHandler : IRequestHandler<CancelAppointment, CommandContext>
    {
        private readonly IAppointmentRepository _appointments;

        public CancelAppointmentHandler(IAppointmentRepository appointments)
        {
            _appointments = appointments;
        }

        public async Task<CommandContext> Handle(CancelAppointment request, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }
    }
}
