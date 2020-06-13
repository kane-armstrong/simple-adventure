﻿using MediatR;
using PetDoctor.Domain.Aggregates.Appointments;
using System.Threading;
using System.Threading.Tasks;

namespace PetDoctor.API.Application.Commands
{
    public class CancelAppointmentHandler : IRequestHandler<CancelAppointment, CommandResult>
    {
        private readonly IAppointmentRepository _appointments;

        public CancelAppointmentHandler(IAppointmentRepository appointments)
        {
            _appointments = appointments;
        }

        public async Task<CommandResult> Handle(CancelAppointment request, CancellationToken cancellationToken)
        {
            var appointment = await _appointments.Find(request.Id);
            if (appointment == null)
                return new CommandResult(false, null);

            appointment.Cancel(request.Reason);

            await _appointments.Save(appointment);

            return new CommandResult(true, appointment.Id);
        }
    }
}
