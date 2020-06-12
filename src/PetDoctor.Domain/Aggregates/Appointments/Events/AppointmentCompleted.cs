using System;

namespace PetDoctor.Domain.Aggregates.Appointments.Events
{
    public class AppointmentCompleted : DomainEvent
    {
        public Guid AppointmentId { get; }
        public AppointmentMemento Data { get; }

        public AppointmentCompleted(Guid appointmentId, AppointmentMemento data)
        {
            AppointmentId = appointmentId;
            Data = data;
        }
    }
}
