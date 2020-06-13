using System;
using MediatR;

namespace PetDoctor.Domain
{
    public abstract class DomainEvent : INotification
    {
        public Guid Id { get; } = Guid.NewGuid();
    }
}
