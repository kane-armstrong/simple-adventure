using System;

namespace PetDoctor.API.Application.Commands
{
    public class RescheduleAppointment : Command
    {
        internal Guid Id { get; set; }
        public DateTimeOffset NewDate { get; set; }
    }
}
