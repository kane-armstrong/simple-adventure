using System;

namespace PetDoctor.Domain.Aggregates.Appointments.Events
{
    public class AppointmentRejected : DomainEvent
    {
        public Guid AppointmentId { get; }
        public AppointmentMemento Data { get; }

        public AppointmentRejected(Guid appointmentId, AppointmentMemento data)
        {
            AppointmentId = appointmentId;
            Data = data;
        }
    }
}
