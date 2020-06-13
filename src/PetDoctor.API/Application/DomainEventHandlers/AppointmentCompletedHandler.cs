using MediatR;
using PetDoctor.Domain.Aggregates.Appointments.Events;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace PetDoctor.API.Application.DomainEventHandlers
{
    public class AppointmentCompletedHandler : INotificationHandler<AppointmentCompleted>
    {
        public Task Handle(AppointmentCompleted notification, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }
    }
}
