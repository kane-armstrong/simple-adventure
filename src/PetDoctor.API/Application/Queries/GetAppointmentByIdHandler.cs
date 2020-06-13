using System;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using PetDoctor.API.Application.Models;

namespace PetDoctor.API.Application.Queries
{
    public class GetAppointmentByIdHandler : IRequestHandler<GetAppointmentById, AppointmentView>
    {
        public Task<AppointmentView> Handle(GetAppointmentById request, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }
    }
}
