﻿using PetDoctor.Domain.Aggregates.Appointments.Events;
using PetDoctor.Infrastructure;
using System.Threading;
using System.Threading.Tasks;
using PetDoctor.Infrastructure.Cqrs;

namespace PetDoctor.API.Application.DomainEventHandlers;

public class AppointmentMembersCheckedInHandler : IEventHandler<AppointmentMembersCheckedIn>
{
    private readonly PetDoctorContext _db;

    public AppointmentMembersCheckedInHandler(PetDoctorContext db)
    {
        _db = db;
    }

    public async Task Handle(AppointmentMembersCheckedIn notification, CancellationToken cancellationToken)
    {
        var snapshot = await _db.AppointmentSnapshots.FindAsync(notification.AppointmentId);
        snapshot.State = notification.State;
        await _db.SaveChangesAsync(cancellationToken);
    }
}