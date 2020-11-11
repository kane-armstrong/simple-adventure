using System;

namespace PetDoctor.Domain.Aggregates.Appointments.Events
{
    public record AppointmentConfirmed : DomainEvent
    {
        public Guid AppointmentId { get; init; }
        public Guid AttendingVeterinarianId { get; init; }
        public readonly AppointmentState State = AppointmentState.Confirmed;
    }
}
