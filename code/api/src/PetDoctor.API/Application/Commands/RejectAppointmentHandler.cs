using PetDoctor.Domain.Aggregates.Appointments;

namespace PetDoctor.API.Application.Commands;

public class RejectAppointmentHandler
{
    private readonly IAppointmentRepository _appointments;

    public RejectAppointmentHandler(IAppointmentRepository appointments)
    {
        _appointments = appointments;
    }

    public async Task<CommandResult> Handle(RejectAppointment request, CancellationToken cancellationToken)
    {
        var appointment = await _appointments.Find(request.Id);
        if (appointment is null)
            return new()
            {
                ResourceFound = false,
                ResourceId = null
            };

        appointment.Reject(request.Reason);

        await _appointments.Save(appointment);

        return new()
        {
            ResourceFound = true,
            ResourceId = appointment.Id
        };
    }
}