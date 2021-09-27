using MediatR;
using PetDoctor.Domain.Aggregates.Appointments.Events;
using PetDoctor.Infrastructure;
using System.Threading;
using System.Threading.Tasks;

namespace PetDoctor.API.Application.DomainEventHandlers
{
    public class AppointmentConfirmedHandler : INotificationHandler<AppointmentConfirmed>
    {
        private readonly PetDoctorContext _db;

        public AppointmentConfirmedHandler(PetDoctorContext db)
        {
            _db = db;
        }

        public async Task Handle(AppointmentConfirmed notification, CancellationToken cancellationToken)
        {
            var snapshot = await _db.AppointmentSnapshots.FindAsync(notification.AppointmentId);
            snapshot.State = notification.State;
            snapshot.AttendingVeterinarianId = notification.AttendingVeterinarianId;
            await _db.SaveChangesAsync(cancellationToken);
        }
    }
}
