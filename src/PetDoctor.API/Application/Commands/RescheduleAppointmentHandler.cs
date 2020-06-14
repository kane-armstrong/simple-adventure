using MediatR;
using PetDoctor.Domain.Aggregates.Appointments;
using System.Threading;
using System.Threading.Tasks;

namespace PetDoctor.API.Application.Commands
{
    public class RescheduleAppointmentHandler : IRequestHandler<RescheduleAppointment, CommandResult>
    {
        private readonly IAppointmentRepository _appointments;

        public RescheduleAppointmentHandler(IAppointmentRepository appointments)
        {
            _appointments = appointments;
        }

        public async Task<CommandResult> Handle(RescheduleAppointment request, CancellationToken cancellationToken)
        {
            var appointment = await _appointments.Find(request.Id);
            if (appointment == null)
                return new CommandResult(false, null);

            appointment.Reschedule(request.NewDate);

            await _appointments.Save(appointment);

            return new CommandResult(true, appointment.Id);
        }
    }
}
