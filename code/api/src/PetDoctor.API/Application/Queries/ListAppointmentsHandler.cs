using MediatR;
using Microsoft.EntityFrameworkCore;
using PetDoctor.API.Application.Models;
using PetDoctor.Infrastructure;
using PetDoctor.Infrastructure.Collections;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace PetDoctor.API.Application.Queries;

public class ListAppointmentsHandler : IRequestHandler<ListAppointments, PaginatedList<AppointmentView>>
{
    private readonly PetDoctorContext _db;

    public ListAppointmentsHandler(PetDoctorContext db)
    {
        _db = db;
    }

    public async Task<PaginatedList<AppointmentView>> Handle(ListAppointments request, CancellationToken cancellationToken)
    {
        var query = _db.AppointmentSnapshots
            .AsNoTracking()
            .Select(snapshot => new AppointmentView
            {
                Id = snapshot.Id,
                ScheduledOn = snapshot.ScheduledOn,
                AttendingVeterinarianId = snapshot.AttendingVeterinarianId,
                RejectionReason = snapshot.RejectionReason,
                CancellationReason = snapshot.CancellationReason,
                Owner = snapshot.Owner,
                Pet = snapshot.Pet,
                ReasonForVisit = snapshot.ReasonForVisit,
                State = snapshot.State.ToString()
            })
            .OrderBy(x => x.ScheduledOn);

        var count = await query.CountAsync(cancellationToken);
        var page = await query.Paginate(request.PageIndex, request.PageSize)
            .ToListAsync(cancellationToken);

        return new PaginatedList<AppointmentView>(page, count, request.PageIndex, request.PageSize);
    }
}