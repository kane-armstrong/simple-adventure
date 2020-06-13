using System;
using MediatR;

namespace PetDoctor.API.Application.Commands
{
    public class CheckinToAppointment : IRequest
    {
        internal Guid Id { get; set; }
    }
}
