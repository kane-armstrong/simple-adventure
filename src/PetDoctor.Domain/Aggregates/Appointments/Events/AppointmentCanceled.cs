using System;

namespace PetDoctor.Domain.Aggregates.Appointments.Events
{
    public class AppointmentCanceled : DomainEvent
    {
        public Guid AppointmentId { get; }
        public AppointmentMemento Data { get; }

        public AppointmentCanceled(Guid appointmentId, AppointmentMemento data)
        {
            AppointmentId = appointmentId;
            Data = data;
        }
    }
}
