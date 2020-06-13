using System;
using MediatR;

namespace PetDoctor.API.Application.Commands
{
    public class CompleteAppointment : IRequest
    {
        internal Guid Id { get; set; }
    }
}
