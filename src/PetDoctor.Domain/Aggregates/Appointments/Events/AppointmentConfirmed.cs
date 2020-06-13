using System;

namespace PetDoctor.Domain.Aggregates.Appointments.Events
{
    public class AppointmentConfirmed : DomainEvent
    {
        public Guid AppointmentId { get; }
        public Guid AttendingVeterinarianId { get; set; }

        public AppointmentConfirmed(Guid appointmentId, Guid attendingVeterinarianId)
        {
            AppointmentId = appointmentId;
            AttendingVeterinarianId = attendingVeterinarianId;
        }
    }
}
