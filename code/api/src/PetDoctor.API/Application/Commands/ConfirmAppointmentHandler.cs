using Microsoft.AspNetCore.Mvc;
using PetDoctor.API.Application.Errors;
using PetDoctor.API.Application.Links;
using PetDoctor.Domain.Aggregates.Appointments;

namespace PetDoctor.API.Application.Commands;

public class ConfirmAppointmentHandler
{
    private readonly IAppointmentRepository _appointments;
    private readonly IAppointmentLinksGenerator _appointmentLinksGenerator;

    public ConfirmAppointmentHandler(
        IAppointmentRepository appointments,
        IAppointmentLinksGenerator appointmentLinksGenerator)
    {
        _appointments = appointments;
        _appointmentLinksGenerator = appointmentLinksGenerator;
    }

    public async Task<CommandResult<Unit, ProblemDetails>> Handle(ConfirmAppointment request, CancellationToken cancellationToken)
    {
        var self = _appointmentLinksGenerator.GenerateSelfLink(request.Id);

        var appointment = await _appointments.Find(request.Id, cancellationToken);
        if (appointment is null)
        {
            return CommandResult.Failed<Unit, ProblemDetails>(Problems.NotFoundProblem(self));
        }

        appointment.Confirm(request.AttendingVeterinarianId);

        await _appointments.Save(appointment, cancellationToken);

        return CommandResult.Success<Unit, ProblemDetails>(Unit.Value);
    }
}