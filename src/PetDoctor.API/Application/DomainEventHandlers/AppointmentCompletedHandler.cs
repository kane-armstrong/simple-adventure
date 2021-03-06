﻿using MediatR;
using PetDoctor.Domain.Aggregates.Appointments.Events;
using PetDoctor.Infrastructure;
using System.Threading;
using System.Threading.Tasks;

namespace PetDoctor.API.Application.DomainEventHandlers
{
    public class AppointmentCompletedHandler : INotificationHandler<AppointmentCompleted>
    {
        private readonly PetDoctorContext _db;

        public AppointmentCompletedHandler(PetDoctorContext db)
        {
            _db = db;
        }

        public async Task Handle(AppointmentCompleted notification, CancellationToken cancellationToken)
        {
            var snapshot = await _db.AppointmentSnapshots.FindAsync(notification.AppointmentId);
            snapshot.State = notification.State;
            await _db.SaveChangesAsync(cancellationToken);
        }
    }
}
