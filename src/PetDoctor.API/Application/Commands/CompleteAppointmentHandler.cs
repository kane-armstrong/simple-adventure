using MediatR;
using PetDoctor.Domain.Aggregates.Appointments;
using System.Threading;
using System.Threading.Tasks;

namespace PetDoctor.API.Application.Commands
{
    public class CompleteAppointmentHandler : IRequestHandler<CompleteAppointment, CommandResult>
    {
        private readonly IAppointmentRepository _appointments;

        public CompleteAppointmentHandler(IAppointmentRepository appointments)
        {
            _appointments = appointments;
        }

        public async Task<CommandResult> Handle(CompleteAppointment request, CancellationToken cancellationToken)
        {
            var appointment = await _appointments.Find(request.Id);
            if (appointment == null)
                return new CommandResult(false, null);

            appointment.Complete();

            await _appointments.Save(appointment);

            return new CommandResult(true, appointment.Id);
        }
    }
}
