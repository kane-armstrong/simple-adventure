using System;

namespace PetDoctor.Domain.Aggregates.Appointments.Events
{
    public record AppointmentRescheduled(Guid AppointmentId, DateTimeOffset Date) : DomainEvent
    {
        public readonly AppointmentState State = AppointmentState.Requested;
    }
}
