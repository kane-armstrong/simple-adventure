using System;

namespace PetDoctor.Domain.Aggregates.Appointments.Events
{
    public record AppointmentMembersCheckedIn : DomainEvent
    {
        public Guid AppointmentId { get; }
        public readonly AppointmentState State = AppointmentState.CheckedIn;

        public AppointmentMembersCheckedIn(Guid appointmentId)
        {
            AppointmentId = appointmentId;
        }
    }
}
