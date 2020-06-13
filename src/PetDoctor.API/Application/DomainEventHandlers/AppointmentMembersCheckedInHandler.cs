using MediatR;
using PetDoctor.Domain.Aggregates.Appointments.Events;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace PetDoctor.API.Application.DomainEventHandlers
{
    public class AppointmentMembersCheckedInHandler : INotificationHandler<AppointmentMembersCheckedIn>
    {
        public Task Handle(AppointmentMembersCheckedIn notification, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }
    }
}
