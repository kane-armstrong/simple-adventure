namespace PetDoctor.Domain.Aggregates.Appointments;

public record AppointmentMemento
{
    public Pet Pet { get; init; }
    public Owner Owner { get; init; }
    public Guid? AttendingVeterinarianId { get; init; }
    public string ReasonForVisit { get; init; }
    public DateTimeOffset ScheduledOn { get; init; }
    public AppointmentState State { get; init; }
    public string CancellationReason { get; init; }
    public string RejectionReason { get; init; }
}