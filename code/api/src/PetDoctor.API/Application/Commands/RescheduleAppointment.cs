using System;

namespace PetDoctor.API.Application.Commands;

public record RescheduleAppointment
{
    internal Guid Id { get; set; }
    public DateTimeOffset NewDate { get; init; }
}