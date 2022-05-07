namespace PetDoctor.Domain.Aggregates.Appointments.Events;

public record AppointmentConfirmed(Guid AppointmentId, Guid AttendingVeterinarianId) : DomainEvent
{
    public readonly AppointmentState State = AppointmentState.Confirmed;
}