using System;
using MediatR;

namespace PetDoctor.API.Application.Commands
{
    public class CancelAppointment : IRequest
    {
        internal Guid Id { get; set; }
    }
}
