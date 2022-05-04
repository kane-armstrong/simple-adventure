namespace PetDoctor.Domain.Aggregates.Appointments.Events;

public record AppointmentCompleted(Guid AppointmentId) : DomainEvent
{
    public readonly AppointmentState State = AppointmentState.Completed;
}