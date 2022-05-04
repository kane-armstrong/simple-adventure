using Microsoft.EntityFrameworkCore;
using PetDoctor.Domain.Aggregates.Appointments.Events;
using PetDoctor.Infrastructure;
using PetDoctor.Infrastructure.Cqrs;

namespace PetDoctor.API.Application.DomainEventHandlers;

public class AppointmentCompletedHandler : IEventHandler<AppointmentCompleted>
{
    private readonly PetDoctorContext _db;

    public AppointmentCompletedHandler(PetDoctorContext db)
    {
        _db = db;
    }

    public async Task Handle(AppointmentCompleted notification, CancellationToken cancellationToken)
    {
        var snapshot = await _db.AppointmentSnapshots.FirstAsync(x => x.Id == notification.AppointmentId, cancellationToken);
        snapshot.State = notification.State;
        await _db.SaveChangesAsync(cancellationToken);
    }
}