using AutoFixture;
using FluentAssertions;
using PetDoctor.Domain.Aggregates.Appointments;
using Xunit;

namespace PetDoctor.API.UnitTests.Domain.Aggregates.Appointments.AppointmentSpec;

public class Rejecting_an_appointment
{
    [Fact]
    public void should_update_state_to_rejected()
    {
        var fixture = new Fixture();
        var sut = fixture.Create<Appointment>();

        sut.Reject("nobody available at the requested time");

        sut.State.Should().Be(AppointmentState.Rejected);
    }

    [Fact]
    public void should_set_rejection_reason_correctly()
    {
        var fixture = new Fixture();
        var sut = fixture.Create<Appointment>();

        const string reason = "sorry but there is nobody available";

        sut.Reject(reason);

        sut.RejectionReason.Should().Be(reason);
    }
}