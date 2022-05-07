using PetDoctor.Domain.Aggregates.Appointments;

namespace PetDoctor.API.Application.Models;

public record AppointmentView
{
    public Guid Id { get; init; }
    public Pet Pet { get; init; } = null!;
    public Owner Owner { get; init; } = null!;
    public Guid? AttendingVeterinarianId { get; init; }
    public string ReasonForVisit { get; init; } = string.Empty;
    public string? RejectionReason { get; init; }
    public string? CancellationReason { get; init; }
    public DateTimeOffset ScheduledOn { get; init; }
    public string State { get; init; } = AppointmentState.Requested.ToString();
}