using MediatR;
using PetDoctor.Domain.Aggregates.Appointments.Events;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace PetDoctor.API.Application.DomainEventHandlers
{
    public class AppointmentRejectedHandler : INotificationHandler<AppointmentRejected>
    {
        public Task Handle(AppointmentRejected notification, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }
    }
}
