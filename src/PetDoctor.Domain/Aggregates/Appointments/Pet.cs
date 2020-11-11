using System;

namespace PetDoctor.Domain.Aggregates.Appointments
{
    public class Pet
    {
        public string Name { get; init; }
        public DateTimeOffset DateOfBirth { get; init; }
        public string Breed { get; init; }
    }
}
