﻿using AutoFixture;
using FluentAssertions;
using PetDoctor.Domain.Aggregates.Appointments;
using PetDoctor.Domain.Aggregates.Appointments.Events;
using Xunit;

namespace PetDoctor.API.UnitTests.Domain.Aggregates.Appointments.AppointmentSpec
{
    public class Applying_an_appointment_checked_in_event
    {
        [Fact]
        public void should_update_state_to_checkedin()
        {
            var fixture = new Fixture();
            var sut = fixture.Create<Appointment>();

            var @event = new AppointmentMembersCheckedIn(sut.Id);

            sut.When(@event);

            sut.State.Should().Be(AppointmentState.CheckedIn);
        }
    }
}