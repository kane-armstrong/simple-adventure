using MediatR;
using PetDoctor.Domain.Aggregates.Appointments.Events;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace PetDoctor.API.Application.DomainEventHandlers
{
    public class AppointmentRescheduledHandler : INotificationHandler<AppointmentRescheduled>
    {
        public Task Handle(AppointmentRescheduled notification, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }
    }
}
