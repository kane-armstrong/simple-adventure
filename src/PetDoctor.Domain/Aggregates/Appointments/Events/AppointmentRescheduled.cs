using System;

namespace PetDoctor.Domain.Aggregates.Appointments.Events
{
    public record AppointmentRescheduled : DomainEvent
    {
        public Guid AppointmentId { get; }
        public DateTimeOffset Date { get; }
        public readonly AppointmentState State = AppointmentState.Requested;

        public AppointmentRescheduled(Guid appointmentId, DateTimeOffset date)
        {
            AppointmentId = appointmentId;
            Date = date;
        }
    }
}
