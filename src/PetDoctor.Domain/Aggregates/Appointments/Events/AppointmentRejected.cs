using System;

namespace PetDoctor.Domain.Aggregates.Appointments.Events
{
    public record AppointmentRejected(Guid AppointmentId, string RejectionReason) : DomainEvent
    {
        public readonly AppointmentState State = AppointmentState.Rejected;
    }
}
