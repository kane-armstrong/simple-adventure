using System;

namespace PetDoctor.Domain.Aggregates.Appointments;

public record Pet(string Name, DateTimeOffset DateOfBirth, string Breed)
{
}