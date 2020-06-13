using System;

namespace PetDoctor.API.Application.Commands
{
    public class ConfirmAppointment : Command
    {
        internal Guid Id { get; set; }
    }
}
