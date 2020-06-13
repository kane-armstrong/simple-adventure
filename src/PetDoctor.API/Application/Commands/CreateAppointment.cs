using MediatR;
using PetDoctor.API.Application.Models;

namespace PetDoctor.API.Application.Commands
{
    public class CreateAppointment : IRequest<AppointmentView>
    {
    }
}
