using PetDoctor.Domain.Aggregates.Appointments;
using System;

namespace PetDoctor.API.Application.Models
{
    public record AppointmentView
    {
        public Guid Id { get; init; }
        public Pet Pet { get; init; }
        public Owner Owner { get; init; }
        public Guid? AttendingVeterinarianId { get; init; }
        public string ReasonForVisit { get; init; } = string.Empty;
        public string? RejectionReason { get; init; }
        public string? CancellationReason { get; init; }
        public DateTimeOffset ScheduledOn { get; init; }
        public string State { get; init; }
    }
}
