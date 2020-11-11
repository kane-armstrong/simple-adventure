using System;

namespace PetDoctor.Domain.Aggregates.Appointments.Events
{
    public record AppointmentRejected : DomainEvent
    {
        public Guid AppointmentId { get; init; }
        public string RejectionReason { get; init; } = string.Empty;
        public readonly AppointmentState State = AppointmentState.Rejected;
    }
}
