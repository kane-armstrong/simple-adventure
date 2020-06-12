using System;

namespace PetDoctor.Domain.Aggregates.Appointments.Events
{
    public class AppointmentMembersCheckedIn : DomainEvent
    {
        public Guid AppointmentId { get; }
        public AppointmentMemento Data { get; }

        public AppointmentMembersCheckedIn(Guid appointmentId, AppointmentMemento data)
        {
            AppointmentId = appointmentId;
            Data = data;
        }
    }
}
