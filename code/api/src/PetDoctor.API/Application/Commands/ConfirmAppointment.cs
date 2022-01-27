using System;

namespace PetDoctor.API.Application.Commands;

public record ConfirmAppointment
{
    internal Guid Id { get; set; }
    public Guid AttendingVeterinarianId { get; init; }
}