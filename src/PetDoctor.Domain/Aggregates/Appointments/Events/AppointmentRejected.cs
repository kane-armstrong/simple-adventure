using System;

namespace PetDoctor.Domain.Aggregates.Appointments.Events
{
    public record AppointmentRejected : DomainEvent
    {
        public Guid AppointmentId { get; }
        public string RejectionReason { get; }
        public readonly AppointmentState State = AppointmentState.Rejected;

        public AppointmentRejected(Guid appointmentId, string rejectionReason)
        {
            AppointmentId = appointmentId;
            RejectionReason = rejectionReason;
        }
    }
}
