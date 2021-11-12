namespace PetDoctor.Domain;

using MediatR;
using System;

public abstract record DomainEvent : INotification
{
    public Guid Id { get; } = Guid.NewGuid();
}