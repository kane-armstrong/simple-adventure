using System;
using MediatR;

namespace PetDoctor.API.Application.Commands
{
    public class CancelAppointment : IRequest<CommandResult>
    {
        internal Guid Id { get; set; }
        public string Reason { get; init; } = string.Empty;
    }
}
