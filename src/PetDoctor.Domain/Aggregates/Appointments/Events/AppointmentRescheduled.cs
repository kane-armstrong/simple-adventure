using System;

namespace PetDoctor.Domain.Aggregates.Appointments.Events
{
    public record AppointmentRescheduled : DomainEvent
    {
        public Guid AppointmentId { get; init; }
        public DateTimeOffset Date { get; init; }
        public readonly AppointmentState State = AppointmentState.Requested;
    }
}
