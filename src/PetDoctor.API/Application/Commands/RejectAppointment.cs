using System;
using MediatR;

namespace PetDoctor.API.Application.Commands
{
    public class RejectAppointment : IRequest
    {
        internal Guid Id { get; set; }
    }
}
