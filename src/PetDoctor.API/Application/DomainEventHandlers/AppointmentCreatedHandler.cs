using MediatR;
using PetDoctor.Domain.Aggregates.Appointments.Events;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace PetDoctor.API.Application.DomainEventHandlers
{
    public class AppointmentCreatedHandler : INotificationHandler<AppointmentCreated>
    {
        public Task Handle(AppointmentCreated notification, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }
    }
}
