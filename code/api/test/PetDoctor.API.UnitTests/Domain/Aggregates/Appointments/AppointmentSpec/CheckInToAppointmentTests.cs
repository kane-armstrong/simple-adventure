using AutoFixture;
using FluentAssertions;
using PetDoctor.Domain.Aggregates.Appointments;
using Xunit;

namespace PetDoctor.API.UnitTests.Domain.Aggregates.Appointments.AppointmentSpec;

public class CheckInTests
{
    [Fact]
    public void Checking_in_to_an_appointment_should_update_state_to_checkedin()
    {
        var fixture = new Fixture();
        var sut = fixture.Create<Appointment>();

        sut.CheckIn();

        sut.State.Should().Be(AppointmentState.CheckedIn);
    }
}