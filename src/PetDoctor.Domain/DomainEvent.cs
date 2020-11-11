using System;
using MediatR;

namespace PetDoctor.Domain
{
    public abstract record DomainEvent : INotification
    {
        public Guid Id { get; } = Guid.NewGuid();
    }
}
