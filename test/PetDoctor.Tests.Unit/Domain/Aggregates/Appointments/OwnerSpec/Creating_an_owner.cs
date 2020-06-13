using FluentAssertions;
using PetDoctor.Domain.Aggregates.Appointments;
using Xunit;

namespace PetDoctor.Tests.Unit.Domain.Aggregates.Appointments.OwnerSpec
{
    public class Creating_an_owner
    {
        [Fact]
        public void should_set_first_name_correctly()
        {
            const string firstName = "Kane";

            var sut = new Owner(firstName, "lastName", "phone", "email");

            sut.FirstName.Should().Be(firstName);
        }

        [Fact]
        public void should_set_last_name_correctly()
        {
            const string lastName = "Armstrong";

            var sut = new Owner("firstName", lastName, "phone", "email");

            sut.LastName.Should().Be(lastName);
        }

        [Fact]
        public void should_set_phone_correctly()
        {
            const string phone = "212-000-0000";

            var sut = new Owner("firstName", "lastName", phone, "email");

            sut.Phone.Should().Be(phone);
        }

        [Fact]
        public void should_set_email_correctly()
        {
            const string email = "kane@somewhere.com";

            var sut = new Owner("firstName", "lastName", "phone", email);

            sut.Email.Should().Be(email);
        }
    }
}
