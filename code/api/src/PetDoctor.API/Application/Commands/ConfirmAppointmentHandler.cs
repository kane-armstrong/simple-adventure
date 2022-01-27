using PetDoctor.Domain.Aggregates.Appointments;
using System.Threading;
using System.Threading.Tasks;

namespace PetDoctor.API.Application.Commands;

public class ConfirmAppointmentHandler
{
    private readonly IAppointmentRepository _appointments;

    public ConfirmAppointmentHandler(IAppointmentRepository appointments)
    {
        _appointments = appointments;
    }

    public async Task<CommandResult> Handle(ConfirmAppointment request, CancellationToken cancellationToken)
    {
        var appointment = await _appointments.Find(request.Id);
        if (appointment is null)
            return new()
            {
                ResourceFound = false,
                ResourceId = null
            };

        appointment.Confirm(request.AttendingVeterinarianId);

        await _appointments.Save(appointment);

        return new()
        {
            ResourceFound = true,
            ResourceId = appointment.Id
        };
    }
}