using System;

namespace PetDoctor.Domain.Aggregates.Appointments.Events
{
    public record AppointmentCreated : DomainEvent
    {
        public Guid AppointmentId { get; }
        public AppointmentMemento Data { get; }

        public AppointmentCreated(Guid id, AppointmentMemento data) => (AppointmentId, Data) = (id, data);
    }
}
