using MediatR;
using PetDoctor.Domain.Aggregates.Appointments.Events;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace PetDoctor.API.Application.DomainEventHandlers
{
    public class AppointmentCanceledHandler : INotificationHandler<AppointmentCanceled>
    {
        public Task Handle(AppointmentCanceled notification, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }
    }
}
