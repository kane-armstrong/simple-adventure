using System;

namespace PetDoctor.Domain.Aggregates.Appointments.Events
{
    public record AppointmentCreated(Guid AppointmentId, AppointmentMemento Data) : DomainEvent
    {
    }
}
