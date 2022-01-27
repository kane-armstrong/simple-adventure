using PetDoctor.Domain.Aggregates.Appointments;
using System.Threading;
using System.Threading.Tasks;

namespace PetDoctor.API.Application.Commands;

public class CheckinToAppointmentHandler
{
    private readonly IAppointmentRepository _appointments;

    public CheckinToAppointmentHandler(IAppointmentRepository appointments)
    {
        _appointments = appointments;
    }

    public async Task<CommandResult> Handle(CheckinToAppointment request, CancellationToken cancellationToken)
    {
        var appointment = await _appointments.Find(request.Id);
        if (appointment is null)
            return new()
            {
                ResourceFound = false,
                ResourceId = null
            };

        appointment.CheckIn();

        await _appointments.Save(appointment);

        return new()
        {
            ResourceFound = true,
            ResourceId = appointment.Id
        };
    }
}