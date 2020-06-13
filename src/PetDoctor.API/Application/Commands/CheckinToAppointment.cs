using System;

namespace PetDoctor.API.Application.Commands
{
    public class CheckinToAppointment : Command
    {
        internal Guid Id { get; set; }
    }
}
