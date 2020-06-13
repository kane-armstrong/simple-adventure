using System;

namespace PetDoctor.API.Application.Commands
{
    public class CompleteAppointment : Command
    {
        internal Guid Id { get; set; }
    }
}
