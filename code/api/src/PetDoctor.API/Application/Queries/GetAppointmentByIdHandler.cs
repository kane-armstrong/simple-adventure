using PetDoctor.API.Application.Extensions;
using PetDoctor.API.Application.Models;
using PetDoctor.Infrastructure;
using System.Threading;
using System.Threading.Tasks;

namespace PetDoctor.API.Application.Queries;

public class GetAppointmentByIdHandler
{
    private readonly PetDoctorContext _db;

    public GetAppointmentByIdHandler(PetDoctorContext db)
    {
        _db = db;
    }

    public async Task<AppointmentView> Handle(GetAppointmentById request, CancellationToken cancellationToken)
    {
        var snapshot = await _db.AppointmentSnapshots.FindAsync(request.Id);
        return snapshot?.ToAppointmentView();
    }
}