﻿namespace PetDoctor.Domain.Aggregates.Appointments.Events;

public record AppointmentMembersCheckedIn(Guid AppointmentId) : DomainEvent
{
    public readonly AppointmentState State = AppointmentState.CheckedIn;
}