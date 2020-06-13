using System;

namespace PetDoctor.Domain.Aggregates.Appointments.Events
{
    public class AppointmentMembersCheckedIn : DomainEvent
    {
        public Guid AppointmentId { get; }

        public AppointmentMembersCheckedIn(Guid appointmentId)
        {
            AppointmentId = appointmentId;
        }
    }
}
