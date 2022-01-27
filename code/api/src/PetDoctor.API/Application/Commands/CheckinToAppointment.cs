using System;

namespace PetDoctor.API.Application.Commands;

public record CheckinToAppointment
{
    internal Guid Id { get; set; }
}