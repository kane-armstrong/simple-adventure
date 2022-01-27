using System;

namespace PetDoctor.API.Application.Commands;

public record RejectAppointment
{
    internal Guid Id { get; set; }
    public string Reason { get; init; } = string.Empty;
}