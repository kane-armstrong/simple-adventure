using System;

namespace PetDoctor.Domain.Aggregates.Appointments.Events
{
    public class AppointmentCreated : DomainEvent
    {
        public Guid AppointmentId { get; }
        public AppointmentMemento Data { get; }

        public AppointmentCreated(Guid appointmentId, AppointmentMemento data)
        {
            AppointmentId = appointmentId;
            Data = data;
        }
    }
}
