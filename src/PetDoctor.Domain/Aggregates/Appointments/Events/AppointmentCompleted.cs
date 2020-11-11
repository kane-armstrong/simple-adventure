using System;

namespace PetDoctor.Domain.Aggregates.Appointments.Events
{
    public record AppointmentCompleted : DomainEvent
    {
        public Guid AppointmentId { get; }
        public readonly AppointmentState State = AppointmentState.Completed;

        public AppointmentCompleted(Guid appointmentId)
        {
            AppointmentId = appointmentId;
        }
    }
}
