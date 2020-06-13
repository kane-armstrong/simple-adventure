using System;
using MediatR;

namespace PetDoctor.API.Application.Commands
{
    public class RescheduleAppointment : IRequest
    {
        internal Guid Id { get; set; }
    }
}
