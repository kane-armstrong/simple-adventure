using System;
using MediatR;

namespace PetDoctor.API.Application.Commands
{
    public class ConfirmAppointment : IRequest
    {
        internal Guid Id { get; set; }
    }
}
