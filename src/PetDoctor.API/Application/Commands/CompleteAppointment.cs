using System;

namespace PetDoctor.API.Application.Commands
{
    public class CompleteAppointment : Command
    {
        public Guid Id { get; set; }
    }
}
