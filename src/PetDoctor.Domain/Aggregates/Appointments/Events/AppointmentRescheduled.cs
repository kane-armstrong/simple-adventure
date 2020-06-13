using System;

namespace PetDoctor.Domain.Aggregates.Appointments.Events
{
    public class AppointmentRescheduled : DomainEvent
    {
        public Guid AppointmentId { get; }
        public DateTimeOffset Date { get; }

        public AppointmentRescheduled(Guid appointmentId, DateTimeOffset date)
        {
            AppointmentId = appointmentId;
            Date = date;
        }
    }
}
