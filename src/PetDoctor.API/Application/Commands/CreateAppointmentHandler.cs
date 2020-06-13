using MediatR;
using System;
using System.Threading;
using System.Threading.Tasks;
using PetDoctor.API.Application.Models;

namespace PetDoctor.API.Application.Commands
{
    public class CreateAppointmentHandler : IRequestHandler<CreateAppointment, AppointmentView>
    {
        public Task<AppointmentView> Handle(CreateAppointment request, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }
    }
}
