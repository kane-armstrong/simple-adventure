using MediatR;
using PetDoctor.Domain.Aggregates.Appointments;
using PetDoctor.Domain.Aggregates.Appointments.Events;
using PetDoctor.Infrastructure;
using System.Threading;
using System.Threading.Tasks;

namespace PetDoctor.API.Application.DomainEventHandlers
{
    public class AppointmentCreatedHandler : INotificationHandler<AppointmentCreated>
    {
        private readonly PetDoctorContext _db;

        public AppointmentCreatedHandler(PetDoctorContext db)
        {
            _db = db;
        }

        public Task Handle(AppointmentCreated notification, CancellationToken cancellationToken)
        {
            _db.AppointmentSnapshots.Add(new AppointmentSnapshot
            {
                Id = notification.AppointmentId,
                State = notification.Data.State,
                ScheduledOn = notification.Data.ScheduledOn,
                AttendingVeterinarianId = notification.Data.AttendingVeterinarianId,
                Owner = notification.Data.Owner,
                Pet = notification.Data.Pet,
                CancellationReason = notification.Data.CancellationReason,
                RejectionReason = notification.Data.RejectionReason,
                ReasonForVisit = notification.Data.ReasonForVisit
            });
            return _db.SaveChangesAsync(cancellationToken);
        }
    }
}
