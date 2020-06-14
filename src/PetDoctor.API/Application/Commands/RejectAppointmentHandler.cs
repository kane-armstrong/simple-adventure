using MediatR;
using PetDoctor.Domain.Aggregates.Appointments;
using System.Threading;
using System.Threading.Tasks;

namespace PetDoctor.API.Application.Commands
{
    public class RejectAppointmentHandler : IRequestHandler<RejectAppointment, CommandResult>
    {
        private readonly IAppointmentRepository _appointments;

        public RejectAppointmentHandler(IAppointmentRepository appointments)
        {
            _appointments = appointments;
        }

        public async Task<CommandResult> Handle(RejectAppointment request, CancellationToken cancellationToken)
        {
            var appointment = await _appointments.Find(request.Id);
            if (appointment == null)
                return new CommandResult(false, null);

            appointment.Reject(request.Reason);

            await _appointments.Save(appointment);

            return new CommandResult(true, appointment.Id);
        }
    }
}
