using AutoFixture;
using FluentAssertions;
using PetDoctor.Domain.Aggregates.Appointments;
using Xunit;

namespace PetDoctor.Tests.Unit.Domain.Aggregates.Appointments.AppointmentSpec
{
    public class Canceling_an_appointment
    {
        [Fact]
        public void should_update_state_to_canceled()
        {
            var fixture = new Fixture();
            var sut = fixture.Create<Appointment>();

            sut.Cancel("i went somewhere else");

            sut.State.Should().Be(AppointmentState.Canceled);
        }

        [Fact]
        public void should_set_cancellation_reason_correctly()
        {
            var fixture = new Fixture();
            var sut = fixture.Create<Appointment>();

            const string reason = "i went somewhere else";

            sut.Cancel(reason);

            sut.CancellationReason.Should().Be(reason);
        }
    }
}
