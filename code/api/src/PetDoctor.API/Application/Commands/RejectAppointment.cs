using System;

namespace PetDoctor.API.Application.Commands;

public record RejectAppointment : Command
{
    internal Guid Id { get; set; }
    public string Reason { get; init; } = string.Empty;
}