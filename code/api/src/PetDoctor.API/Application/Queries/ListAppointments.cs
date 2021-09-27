using MediatR;
using Microsoft.AspNetCore.Mvc;
using PetDoctor.API.Application.Models;
using PetDoctor.Infrastructure.Collections;

namespace PetDoctor.API.Application.Queries
{
    public record ListAppointments : IRequest<PaginatedList<AppointmentView>>
    {
        public const string PageIndexQueryArg = "index";
        public const string PageSizeQueryArg = "size";

        [FromQuery(Name = PageIndexQueryArg)]
        public int PageIndex { get; init; }
        [FromQuery(Name = PageSizeQueryArg)]
        public int PageSize { get; init; }
    }
}
