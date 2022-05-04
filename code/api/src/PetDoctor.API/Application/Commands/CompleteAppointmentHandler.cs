using PetDoctor.Domain.Aggregates.Appointments;

namespace PetDoctor.API.Application.Commands;

public class CompleteAppointmentHandler
{
    private readonly IAppointmentRepository _appointments;

    public CompleteAppointmentHandler(IAppointmentRepository appointments)
    {
        _appointments = appointments;
    }

    public async Task<CommandResult> Handle(CompleteAppointment request, CancellationToken cancellationToken)
    {
        var appointment = await _appointments.Find(request.Id, cancellationToken);
        if (appointment is null)
            return new()
            {
                ResourceFound = false,
                ResourceId = null
            };

        appointment.Complete();

        await _appointments.Save(appointment, cancellationToken);

        return new()
        {
            ResourceFound = true,
            ResourceId = appointment.Id
        };
    }
}