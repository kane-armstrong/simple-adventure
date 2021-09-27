using System;

namespace PetDoctor.API.Application.Commands
{
    public record RescheduleAppointment : Command
    {
        internal Guid Id { get; set; }
        public DateTimeOffset NewDate { get; init; }
    }
}
