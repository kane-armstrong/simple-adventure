using System;

namespace PetDoctor.Domain.Aggregates.Appointments.Events
{
    public class AppointmentRescheduled : DomainEvent
    {
        public Guid AppointmentId { get; }
        public AppointmentMemento Data { get; }

        public AppointmentRescheduled(Guid appointmentId, AppointmentMemento data)
        {
            AppointmentId = appointmentId;
            Data = data;
        }
    }
}
