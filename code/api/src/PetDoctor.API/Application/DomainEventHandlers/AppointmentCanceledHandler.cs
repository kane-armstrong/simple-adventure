﻿using Microsoft.EntityFrameworkCore;
using PetDoctor.Domain.Aggregates.Appointments.Events;
using PetDoctor.Infrastructure;
using PetDoctor.Infrastructure.Cqrs;

namespace PetDoctor.API.Application.DomainEventHandlers;

public class AppointmentCanceledHandler : IEventHandler<AppointmentCanceled>
{
    private readonly PetDoctorContext _db;

    public AppointmentCanceledHandler(PetDoctorContext db)
    {
        _db = db;
    }

    public async Task Handle(AppointmentCanceled notification, CancellationToken cancellationToken)
    {
        var snapshot = await _db.AppointmentSnapshots.FirstAsync(x => x.Id == notification.AppointmentId, cancellationToken);
        snapshot.CancellationReason = notification.CancellationReason;
        snapshot.State = notification.State;
        await _db.SaveChangesAsync(cancellationToken);
    }
}