using System;

namespace PetDoctor.Domain.Aggregates.Appointments.Events
{
    public record AppointmentCompleted : DomainEvent
    {
        public Guid AppointmentId { get; init; }
        public readonly AppointmentState State = AppointmentState.Completed;
    }
}
