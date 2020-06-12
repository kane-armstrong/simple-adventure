using System;

namespace PetDoctor.Domain
{
    public abstract class DomainEvent
    {
        public Guid Id { get; } = Guid.NewGuid();
    }
}
