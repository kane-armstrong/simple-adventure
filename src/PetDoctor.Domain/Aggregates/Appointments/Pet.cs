using System;

namespace PetDoctor.Domain.Aggregates.Appointments
{
    public class Pet
    {
        public string Name { get; }
        public DateTimeOffset DateOfBirth { get; }
        public string Breed { get; }

        public Pet(string name, DateTimeOffset dateOfBirth, string breed)
        {
            Name = name;
            DateOfBirth = dateOfBirth;
            Breed = breed;
        }
    }
}
