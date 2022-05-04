using Microsoft.AspNetCore.Mvc;
using PetDoctor.API.Application.Errors;
using PetDoctor.API.Application.Links;
using PetDoctor.Domain.Aggregates.Appointments;

namespace PetDoctor.API.Application.Commands;

public class CheckinToAppointmentHandler
{
    private readonly IAppointmentRepository _appointments;
    private readonly IAppointmentLinksGenerator _appointmentLinksGenerator;

    public CheckinToAppointmentHandler(
        IAppointmentRepository appointments,
        IAppointmentLinksGenerator appointmentLinksGenerator)
    {
        _appointments = appointments;
        _appointmentLinksGenerator = appointmentLinksGenerator;
    }

    public async Task<CommandResult<Unit, ProblemDetails>> Handle(CheckinToAppointment request, CancellationToken cancellationToken)
    {
        var self = _appointmentLinksGenerator.GenerateSelfLink(request.Id);

        var appointment = await _appointments.Find(request.Id, cancellationToken);
        if (appointment is null)
        {
            return CommandResult.Failed<Unit, ProblemDetails>(Problems.NotFoundProblem(self));
        }

        appointment.CheckIn();

        await _appointments.Save(appointment, cancellationToken);

        return CommandResult.Success<Unit, ProblemDetails>(Unit.Value);
    }
}