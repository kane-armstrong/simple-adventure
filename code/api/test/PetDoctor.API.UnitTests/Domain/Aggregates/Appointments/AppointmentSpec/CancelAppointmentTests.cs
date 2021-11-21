using AutoFixture;
using FluentAssertions;
using PetDoctor.Domain.Aggregates.Appointments;
using Xunit;

namespace PetDoctor.API.UnitTests.Domain.Aggregates.Appointments.AppointmentSpec;

public class CancelAppointmentTests
{
    [Fact]
    public void Canceling_an_appointment_should_update_state_to_canceled()
    {
        var fixture = new Fixture();
        var sut = fixture.Create<Appointment>();

        sut.Cancel("i went somewhere else");

        sut.State.Should().Be(AppointmentState.Canceled);
    }

    [Fact]
    public void Canceling_an_appointment_should_set_cancellation_reason_correctly()
    {
        var fixture = new Fixture();
        var sut = fixture.Create<Appointment>();

        const string reason = "i went somewhere else";

        sut.Cancel(reason);

        sut.CancellationReason.Should().Be(reason);
    }
}