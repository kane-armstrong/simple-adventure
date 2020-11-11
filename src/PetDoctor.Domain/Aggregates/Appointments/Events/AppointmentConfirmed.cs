using System;

namespace PetDoctor.Domain.Aggregates.Appointments.Events
{
    public record AppointmentConfirmed : DomainEvent
    {
        public Guid AppointmentId { get; }
        public Guid AttendingVeterinarianId { get; }
        public readonly AppointmentState State = AppointmentState.Confirmed;

        public AppointmentConfirmed(Guid appointmentId, Guid attendingVeterinarianId)
        {
            AppointmentId = appointmentId;
            AttendingVeterinarianId = attendingVeterinarianId;
        }
    }
}
