using System;

namespace PetDoctor.API.Application.Commands
{
    public record CompleteAppointment : Command
    {
        internal Guid Id { get; set; }
    }
}
