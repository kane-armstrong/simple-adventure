using MediatR;
using PetDoctor.API.Application.Models;
using PetDoctor.Infrastructure.Collections;

namespace PetDoctor.API.Application.Queries
{
    public class ListAppointments : IRequest<PaginatedList<AppointmentView>>
    {
    }
}
