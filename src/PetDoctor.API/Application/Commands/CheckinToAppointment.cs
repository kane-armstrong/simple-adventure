using System;

namespace PetDoctor.API.Application.Commands
{
    public class CheckinToAppointment : Command
    {
        public Guid Id { get; set; }
    }
}
