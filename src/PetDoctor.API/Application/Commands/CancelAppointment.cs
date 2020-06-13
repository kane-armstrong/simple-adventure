using System;

namespace PetDoctor.API.Application.Commands
{
    public class CancelAppointment : Command
    {
        internal Guid Id { get; set; }
    }
}
