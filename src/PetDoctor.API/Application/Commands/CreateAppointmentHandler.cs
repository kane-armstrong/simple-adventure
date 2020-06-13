using MediatR;
using PetDoctor.Domain.Aggregates.Appointments;
using System.Threading;
using System.Threading.Tasks;

namespace PetDoctor.API.Application.Commands
{
    public class CreateAppointmentHandler : IRequestHandler<CreateAppointment, CommandResult>
    {
        private readonly IAppointmentRepository _appointments;

        public CreateAppointmentHandler(IAppointmentRepository appointments)
        {
            _appointments = appointments;
        }

        public async Task<CommandResult> Handle(CreateAppointment request, CancellationToken cancellationToken)
        {
            var pet = new Pet(request.PetName, request.PetDateOfBirth, request.PetBreed);
            var owner = new Owner(request.OwnerFirstName, request.OwnerLastName, request.OwnerPhone, request.OwnerEmail);
            var appointment = new Appointment(pet, owner, request.DesiredVerterinarianId, request.ReasonForVisit, request.DesiredDate);
            await _appointments.Save(appointment);
            return new CommandResult(true, appointment.Id);
        }
    }
}