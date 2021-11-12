using System.Collections.Generic;
using System.Collections.ObjectModel;
using AutoFixture;
using FluentAssertions;
using PetDoctor.Domain;
using PetDoctor.Domain.Aggregates.Appointments;
using PetDoctor.Domain.Aggregates.Appointments.Events;
using Xunit;

namespace PetDoctor.API.UnitTests.Domain.Aggregates.Appointments.AppointmentSpec
{
    public class Applying_an_appointment_canceled_event
    {
        [Fact]
        public void should_update_state_to_canceled()
        {
            var fixture = new Fixture();
            var sut = fixture.Create<Appointment>();

            var @event = new AppointmentCanceled(sut.Id, "i went somewhere else");

            sut.ReplayEvents(new List<DomainEvent> { @event });

            sut.State.Should().Be(AppointmentState.Canceled);
        }

        [Fact]
        public void should_set_cancellation_reason_correctly()
        {
            var fixture = new Fixture();
            var sut = fixture.Create<Appointment>();

            var @event = new AppointmentCanceled(sut.Id, "i went somewhere else");

            sut.ReplayEvents(new List<DomainEvent> { @event });

            sut.CancellationReason.Should().Be(@event.CancellationReason);
        }
    }
}
