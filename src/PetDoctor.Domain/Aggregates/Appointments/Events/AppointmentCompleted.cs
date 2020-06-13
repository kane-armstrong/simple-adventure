using System;

namespace PetDoctor.Domain.Aggregates.Appointments.Events
{
    public class AppointmentCompleted : DomainEvent
    {
        public Guid AppointmentId { get; }
        public AppointmentState State = AppointmentState.Completed;

        public AppointmentCompleted(Guid appointmentId)
        {
            AppointmentId = appointmentId;
        }
    }
}
