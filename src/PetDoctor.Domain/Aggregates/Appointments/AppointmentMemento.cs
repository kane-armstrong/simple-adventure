using System;

namespace PetDoctor.Domain.Aggregates.Appointments
{
    public class AppointmentMemento
    {
        public Pet Pet { get; set; }
        public Owner Owner { get; set; }
        public Guid? AttendingVeterinarianId { get; set; }
        public string ReasonForVisit { get; set; }
        public DateTimeOffset ScheduledOn { get; set; }
        public AppointmentState State { get; set; }
    }
}
