using PetDoctor.Domain.Aggregates.Appointments;
using System;

namespace PetDoctor.API.Application.Models
{
    public class AppointmentView
    {
        public Guid Id { get; set; }
        public Pet Pet { get; set; }
        public Owner Owner { get; set; }
        public Guid? AttendingVeterinarianId { get; set; }
        public string ReasonForVisit { get; set; } = string.Empty;
        public string? RejectionReason { get; set; }
        public string? CancellationReason { get; set; }
        public DateTimeOffset ScheduledOn { get; set; }
        public string State { get; set; }
    }
}
