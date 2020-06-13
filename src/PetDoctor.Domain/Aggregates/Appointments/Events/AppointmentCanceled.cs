using System;

namespace PetDoctor.Domain.Aggregates.Appointments.Events
{
    public class AppointmentCanceled : DomainEvent
    {
        public Guid AppointmentId { get; }
        public string CancellationReason { get; set; }
        public AppointmentState State = AppointmentState.Canceled;

        public AppointmentCanceled(Guid appointmentId, string cancellationReason)
        {
            AppointmentId = appointmentId;
            CancellationReason = cancellationReason;
        }
    }
}
