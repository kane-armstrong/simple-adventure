using Microsoft.AspNetCore.Mvc;
using PetDoctor.API.Application.Errors;
using PetDoctor.API.Application.Extensions;
using PetDoctor.API.Application.Links;
using PetDoctor.API.Application.Models;
using PetDoctor.Infrastructure;

namespace PetDoctor.API.Application.Queries;

public class GetAppointmentByIdHandler
{
    private readonly PetDoctorContext _db;
    private readonly IAppointmentLinksGenerator _appointmentLinksGenerator;

    public GetAppointmentByIdHandler(PetDoctorContext db, IAppointmentLinksGenerator appointmentLinksGenerator)
    {
        _db = db;
        _appointmentLinksGenerator = appointmentLinksGenerator;
    }

    public async Task<CommandResult<AppointmentView, ProblemDetails>> Handle(GetAppointmentById request)
    {
        var snapshot = await _db.AppointmentSnapshots.FindAsync(request.Id);
        if (snapshot != null)
            return CommandResult.Success<AppointmentView, ProblemDetails>(snapshot.ToAppointmentView());

        var self = _appointmentLinksGenerator.GenerateSelfLink(request.Id);
        return CommandResult.Failed<AppointmentView, ProblemDetails>(Problems.NotFoundProblem(self));
    }
}