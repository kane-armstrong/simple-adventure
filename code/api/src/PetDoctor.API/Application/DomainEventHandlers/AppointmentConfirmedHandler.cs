﻿using Microsoft.EntityFrameworkCore;
using PetDoctor.Domain.Aggregates.Appointments.Events;
using PetDoctor.Infrastructure;
using PetDoctor.Infrastructure.Cqrs;

namespace PetDoctor.API.Application.DomainEventHandlers;

public class AppointmentConfirmedHandler : IEventHandler<AppointmentConfirmed>
{
    private readonly PetDoctorContext _db;

    public AppointmentConfirmedHandler(PetDoctorContext db)
    {
        _db = db;
    }

    public async Task Handle(AppointmentConfirmed notification, CancellationToken cancellationToken)
    {
        var snapshot = await _db.AppointmentSnapshots.FirstAsync(x => x.Id == notification.AppointmentId, cancellationToken);
        snapshot.State = notification.State;
        snapshot.AttendingVeterinarianId = notification.AttendingVeterinarianId;
        await _db.SaveChangesAsync(cancellationToken);
    }
}