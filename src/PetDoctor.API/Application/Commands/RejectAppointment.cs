using System;

namespace PetDoctor.API.Application.Commands
{
    public class RejectAppointment : Command
    {
        internal Guid Id { get; set; }
    }
}
