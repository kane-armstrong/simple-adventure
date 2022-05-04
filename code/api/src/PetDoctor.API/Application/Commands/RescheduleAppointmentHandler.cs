﻿using Microsoft.AspNetCore.Mvc;
using PetDoctor.API.Application.Errors;
using PetDoctor.API.Application.Links;
using PetDoctor.Domain.Aggregates.Appointments;

namespace PetDoctor.API.Application.Commands;

public class RescheduleAppointmentHandler
{
    private readonly IAppointmentRepository _appointments;
    private readonly IAppointmentLinksGenerator _appointmentLinksGenerator;

    public RescheduleAppointmentHandler(
        IAppointmentRepository appointments,
        IAppointmentLinksGenerator appointmentLinksGenerator)
    {
        _appointments = appointments;
        _appointmentLinksGenerator = appointmentLinksGenerator;
    }

    public async Task<CommandResult<Unit, ProblemDetails>> Handle(RescheduleAppointment request, CancellationToken cancellationToken)
    {
        var self = _appointmentLinksGenerator.GenerateSelfLink(request.Id);

        var appointment = await _appointments.Find(request.Id, cancellationToken);
        if (appointment is null)
        {
            return CommandResult.Failed<Unit, ProblemDetails>(Problems.NotFoundProblem(self));
        }

        appointment.Reschedule(request.NewDate);

        await _appointments.Save(appointment, cancellationToken);

        return CommandResult.Success<Unit, ProblemDetails>(Unit.Value);
    }
}