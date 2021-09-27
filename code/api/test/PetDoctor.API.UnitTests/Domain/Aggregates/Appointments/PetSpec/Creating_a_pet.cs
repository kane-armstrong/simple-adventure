using System;
using FluentAssertions;
using PetDoctor.Domain.Aggregates.Appointments;
using Xunit;

namespace PetDoctor.API.UnitTests.Domain.Aggregates.Appointments.PetSpec
{
    public class Creating_a_pet
    {
        [Fact]
        public void should_set_name_correctly()
        {
            const string petName = "Toby";

            var sut = new Pet(petName, DateTimeOffset.Now, "breed");

            sut.Name.Should().Be(petName);
        }

        [Fact]
        public void should_set_date_of_birth_correctly()
        {
            var dob = DateTimeOffset.Parse("2012-07-04T12:00:00.0000000+12:00");

            var sut = new Pet("name", dob, "breed");

            sut.DateOfBirth.Should().Be(dob);
        }

        [Fact]
        public void should_set_breed_correctly()
        {
            const string breed = "Japanese Spitz";

            var sut = new Pet("name", DateTimeOffset.Now, breed);

            sut.Breed.Should().Be(breed);
        }
    }
}
