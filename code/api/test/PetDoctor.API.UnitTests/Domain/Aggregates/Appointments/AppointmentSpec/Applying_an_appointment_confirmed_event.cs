using System;
using AutoFixture;
using FluentAssertions;
using PetDoctor.Domain.Aggregates.Appointments;
using PetDoctor.Domain.Aggregates.Appointments.Events;
using Xunit;

namespace PetDoctor.API.UnitTests.Domain.Aggregates.Appointments.AppointmentSpec
{
    public class Applying_an_appointment_confirmed_event
    {
        [Fact]
        public void should_update_state_to_confirmed()
        {
            var fixture = new Fixture();
            var sut = fixture.Create<Appointment>();

            var @event = new AppointmentConfirmed(sut.Id, Guid.NewGuid());

            sut.Apply(@event);

            sut.State.Should().Be(AppointmentState.Confirmed);
        }

        [Fact]
        public void should_set_vet_id_correctly()
        {
            var fixture = new Fixture();
            var sut = fixture.Create<Appointment>();

            var @event = new AppointmentConfirmed(sut.Id, Guid.NewGuid());

            sut.Apply(@event);

            sut.State.Should().Be(AppointmentState.Confirmed);

            sut.AttendingVeterinarianId.Should().Be(@event.AttendingVeterinarianId);
        }
    }
}
