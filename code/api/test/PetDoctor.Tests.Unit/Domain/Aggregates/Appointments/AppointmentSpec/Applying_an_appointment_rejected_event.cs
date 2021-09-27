using AutoFixture;
using FluentAssertions;
using PetDoctor.Domain.Aggregates.Appointments;
using PetDoctor.Domain.Aggregates.Appointments.Events;
using Xunit;

namespace PetDoctor.Tests.Unit.Domain.Aggregates.Appointments.AppointmentSpec
{
    public class Applying_an_appointment_rejected_event
    {
        [Fact]
        public void should_update_state_to_rejected()
        {
            var fixture = new Fixture();
            var sut = fixture.Create<Appointment>();

            var @event = new AppointmentRejected(sut.Id, "nobody available");

            sut.Apply(@event);

            sut.State.Should().Be(AppointmentState.Rejected);
        }

        [Fact]
        public void should_set_rejection_reason_correctly()
        {
            var fixture = new Fixture();
            var sut = fixture.Create<Appointment>();

            var @event = new AppointmentRejected(sut.Id, "nobody available");

            sut.Apply(@event);

            sut.RejectionReason.Should().Be(@event.RejectionReason);
        }
    }
}
