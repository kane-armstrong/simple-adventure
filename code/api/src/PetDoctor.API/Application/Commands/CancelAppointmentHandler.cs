﻿using PetDoctor.Domain.Aggregates.Appointments;

namespace PetDoctor.API.Application.Commands;

public class CancelAppointmentHandler
{
    private readonly IAppointmentRepository _appointments;

    public CancelAppointmentHandler(IAppointmentRepository appointments)
    {
        _appointments = appointments;
    }

    public async Task<CommandResult> Handle(CancelAppointment request, CancellationToken cancellationToken)
    {
        var appointment = await _appointments.Find(request.Id, cancellationToken);
        if (appointment is null)
            return new()
            {
                ResourceFound = false,
                ResourceId = null
            };

        appointment.Cancel(request.Reason);

        await _appointments.Save(appointment, cancellationToken);

        return new()
        {
            ResourceFound = true,
            ResourceId = appointment.Id
        };
    }
}