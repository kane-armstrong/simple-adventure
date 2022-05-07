namespace PetDoctor.Domain.Aggregates.Appointments.Events;

public record AppointmentCanceled(Guid AppointmentId, string CancellationReason) : DomainEvent
{
    public readonly AppointmentState State = AppointmentState.Canceled;
}