using Microsoft.EntityFrameworkCore;
using PetDoctor.Domain.Aggregates.Appointments.Events;
using PetDoctor.Infrastructure;
using PetDoctor.Infrastructure.Cqrs;

namespace PetDoctor.API.Application.DomainEventHandlers;

public class AppointmentRescheduledHandler : IEventHandler<AppointmentRescheduled>
{
    private readonly PetDoctorContext _db;

    public AppointmentRescheduledHandler(PetDoctorContext db)
    {
        _db = db;
    }

    public async Task Handle(AppointmentRescheduled notification, CancellationToken cancellationToken)
    {
        var snapshot = await _db.AppointmentSnapshots.FirstAsync(x => x.Id == notification.AppointmentId, cancellationToken);
        snapshot.State = notification.State;
        snapshot.ScheduledOn = notification.Date;
        await _db.SaveChangesAsync(cancellationToken);
    }
}
