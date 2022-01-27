using Microsoft.AspNetCore.Mvc;

namespace PetDoctor.API.Application.Queries;

public record ListAppointments
{
    public const string PageIndexQueryArg = "index";
    public const string PageSizeQueryArg = "size";

    [FromQuery(Name = PageIndexQueryArg)]
    public int PageIndex { get; init; }
    [FromQuery(Name = PageSizeQueryArg)]
    public int PageSize { get; init; }
}