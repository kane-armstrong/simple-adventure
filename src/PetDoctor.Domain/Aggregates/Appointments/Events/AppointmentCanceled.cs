using System;

namespace PetDoctor.Domain.Aggregates.Appointments.Events
{
    public record AppointmentCanceled : DomainEvent
    {
        public Guid AppointmentId { get; init; }
        public string CancellationReason { get; init; } = string.Empty;
        public readonly AppointmentState State = AppointmentState.Canceled;
    }
}
