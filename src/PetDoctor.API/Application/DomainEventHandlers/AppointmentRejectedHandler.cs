using MediatR;
using PetDoctor.Domain.Aggregates.Appointments.Events;
using System;
using System.Threading;
using System.Threading.Tasks;
using PetDoctor.Infrastructure;

namespace PetDoctor.API.Application.DomainEventHandlers
{
    public class AppointmentRejectedHandler : INotificationHandler<AppointmentRejected>
    {
        private readonly PetDoctorContext _db;

        public AppointmentRejectedHandler(PetDoctorContext db)
        {
            _db = db;
        }

        public async Task Handle(AppointmentRejected notification, CancellationToken cancellationToken)
        {
            var snapshot = await _db.AppointmentSnapshots.FindAsync(notification.AppointmentId);
            snapshot.State = notification.State;
            snapshot.RejectionReason = notification.RejectionReason;
            await _db.SaveChangesAsync(cancellationToken);
        }
    }
}
