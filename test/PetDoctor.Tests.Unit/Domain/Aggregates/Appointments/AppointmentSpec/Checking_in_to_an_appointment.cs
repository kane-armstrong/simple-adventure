using AutoFixture;
using FluentAssertions;
using PetDoctor.Domain.Aggregates.Appointments;
using Xunit;

namespace PetDoctor.Tests.Unit.Domain.Aggregates.Appointments.AppointmentSpec
{
    public class Checking_in_to_an_appointment
    {
        [Fact]
        public void should_update_state_to_checkedin()
        {
            var fixture = new Fixture();
            var sut = fixture.Create<Appointment>();

            sut.CheckIn();

            sut.State.Should().Be(AppointmentState.CheckedIn);
        }
    }
}
