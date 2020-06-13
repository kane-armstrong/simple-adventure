using MediatR;
using PetDoctor.Domain.Aggregates.Appointments.Events;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace PetDoctor.API.Application.DomainEventHandlers
{
    public class AppointmentConfirmedHandler : INotificationHandler<AppointmentConfirmed>
    {
        public Task Handle(AppointmentConfirmed notification, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }
    }
}
