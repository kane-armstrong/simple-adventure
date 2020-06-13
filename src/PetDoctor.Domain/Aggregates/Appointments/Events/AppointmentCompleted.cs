using System;

namespace PetDoctor.Domain.Aggregates.Appointments.Events
{
    public class AppointmentCompleted : DomainEvent
    {
        public Guid AppointmentId { get; }

        public AppointmentCompleted(Guid appointmentId)
        {
            AppointmentId = appointmentId;
        }
    }
}
