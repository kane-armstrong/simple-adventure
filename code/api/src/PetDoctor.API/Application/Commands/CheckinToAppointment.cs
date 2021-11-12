using System;

namespace PetDoctor.API.Application.Commands;

public record CheckinToAppointment : Command
{
    internal Guid Id { get; set; }
}