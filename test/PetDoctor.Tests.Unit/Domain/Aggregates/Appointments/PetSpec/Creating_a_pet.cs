using System;
using FluentAssertions;
using PetDoctor.Domain.Aggregates.Appointments;
using Xunit;

namespace PetDoctor.Tests.Unit.Domain.Aggregates.Appointments.PetSpec
{
    public class Creating_a_pet
    {
        [Fact]
        public void should_set_name_correctly()
        {
            const string petName = "Toby";

            Pet sut = new()
            {
                Name = petName,
                DateOfBirth = DateTimeOffset.Now,
                Breed = "breed"
            };

            sut.Name.Should().Be(petName);
        }

        [Fact]
        public void should_set_date_of_birth_correctly()
        {
            var dob = DateTimeOffset.Parse("2012-07-04T12:00:00.0000000+12:00");

            var sut = new Pet
            {
                Name = "name",
                DateOfBirth = dob,
                Breed = "breed"
            };

            sut.DateOfBirth.Should().Be(dob);
        }

        [Fact]
        public void should_set_breed_correctly()
        {
            const string breed = "Japanese Spitz";

            var sut = new Pet
            {
                Name = "name",
                DateOfBirth = DateTimeOffset.Now,
                Breed = breed
            };

            sut.Breed.Should().Be(breed);
        }
    }
}
