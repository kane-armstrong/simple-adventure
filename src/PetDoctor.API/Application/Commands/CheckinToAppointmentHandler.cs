using MediatR;
using PetDoctor.Domain.Aggregates.Appointments;
using System.Threading;
using System.Threading.Tasks;

namespace PetDoctor.API.Application.Commands
{
    public class CheckinToAppointmentHandler : IRequestHandler<CheckinToAppointment, CommandResult>
    {
        private readonly IAppointmentRepository _appointments;

        public CheckinToAppointmentHandler(IAppointmentRepository appointments)
        {
            _appointments = appointments;
        }

        public async Task<CommandResult> Handle(CheckinToAppointment request, CancellationToken cancellationToken)
        {
            var appointment = await _appointments.Find(request.Id);
            if (appointment == null)
                return new CommandResult(false, null);

            appointment.CheckIn();

            await _appointments.Save(appointment);

            return new CommandResult(true, appointment.Id);
        }
    }
}
