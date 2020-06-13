using System;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using PetDoctor.API.Application.Models;
using PetDoctor.Infrastructure.Collections;

namespace PetDoctor.API.Application.Queries
{
    public class ListAppointmentsHandler : IRequestHandler<ListAppointments, PaginatedList<AppointmentView>>
    {
        public Task<PaginatedList<AppointmentView>> Handle(ListAppointments request, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }
    }
}
