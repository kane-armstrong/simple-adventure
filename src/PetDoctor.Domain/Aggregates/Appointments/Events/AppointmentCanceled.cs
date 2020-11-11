using System;

namespace PetDoctor.Domain.Aggregates.Appointments.Events
{
    public record AppointmentCanceled : DomainEvent
    {
        public Guid AppointmentId { get; }
        public string CancellationReason { get; }
        public readonly AppointmentState State = AppointmentState.Canceled;

        public AppointmentCanceled(Guid appointmentId, string cancellationReason)
        {
            AppointmentId = appointmentId;
            CancellationReason = cancellationReason;
        }
    }
}
