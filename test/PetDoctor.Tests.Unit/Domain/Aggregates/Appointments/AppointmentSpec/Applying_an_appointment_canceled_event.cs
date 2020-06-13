using AutoFixture;
using FluentAssertions;
using PetDoctor.Domain.Aggregates.Appointments;
using PetDoctor.Domain.Aggregates.Appointments.Events;
using Xunit;

namespace PetDoctor.Tests.Unit.Domain.Aggregates.Appointments.AppointmentSpec
{
    public class Applying_an_appointment_canceled_event
    {
        [Fact]
        public void should_update_state_to_canceled()
        {
            var fixture = new Fixture();
            var sut = fixture.Create<Appointment>();

            var @event = new AppointmentCanceled(sut.Id, "i went somewhere else");

            sut.Apply(@event);

            sut.State.Should().Be(AppointmentState.Canceled);
        }

        [Fact]
        public void should_set_cancellation_reason_correctly()
        {
            var fixture = new Fixture();
            var sut = fixture.Create<Appointment>();

            var @event = new AppointmentCanceled(sut.Id, "i went somewhere else");

            sut.Apply(@event);

            sut.CancellationReason.Should().Be(@event.CancellationReason);
        }
    }
}
