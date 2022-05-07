namespace PetDoctor.Domain.Aggregates.Appointments;

public class AppointmentSnapshot
{
    public Guid Id { get; set; }
    public Pet Pet { get; set; } = null!;
    public Owner Owner { get; set; } = null!;
    public Guid? AttendingVeterinarianId { get; set; }
    public string ReasonForVisit { get; set; } = string.Empty;
    public string? RejectionReason { get; set; }
    public string? CancellationReason { get; set; }
    public DateTimeOffset ScheduledOn { get; set; }
    public AppointmentState State { get; set; }
}