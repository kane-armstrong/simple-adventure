using Microsoft.EntityFrameworkCore;
using PetDoctor.Domain.Aggregates.Appointments.Events;
using PetDoctor.Infrastructure;
using PetDoctor.Infrastructure.Cqrs;

namespace PetDoctor.API.Application.DomainEventHandlers;

public class AppointmentRejectedHandler : IEventHandler<AppointmentRejected>
{
    private readonly PetDoctorContext _db;

    public AppointmentRejectedHandler(PetDoctorContext db)
    {
        _db = db;
    }

    public async Task Handle(AppointmentRejected notification, CancellationToken cancellationToken)
    {
        var snapshot = await _db.AppointmentSnapshots.FirstAsync(x => x.Id == notification.AppointmentId, cancellationToken);
        snapshot.State = notification.State;
        snapshot.RejectionReason = notification.RejectionReason;
        await _db.SaveChangesAsync(cancellationToken);
    }
}