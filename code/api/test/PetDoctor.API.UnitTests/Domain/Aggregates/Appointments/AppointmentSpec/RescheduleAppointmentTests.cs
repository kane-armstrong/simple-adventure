using System;
using AutoFixture;
using FluentAssertions;
using PetDoctor.Domain.Aggregates.Appointments;
using Xunit;

namespace PetDoctor.API.UnitTests.Domain.Aggregates.Appointments.AppointmentSpec;

public class RescheduleTests
{
    [Fact]
    public void Rescheduling_an_appointment_should_update_state_to_requested()
    {
        var fixture = new Fixture();
        var sut = fixture.Create<Appointment>();

        sut.Reschedule(DateTimeOffset.Now.AddDays(3));

        sut.State.Should().Be(AppointmentState.Requested);
    }

    [Fact]
    public void Rescheduling_an_appointment_should_set_scheduled_on_correctly()
    {
        var fixture = new Fixture();
        var sut = fixture.Create<Appointment>();

        var scheduledOn = DateTimeOffset.Now.AddDays(3);
        sut.Reschedule(scheduledOn);

        sut.ScheduledOn.Should().Be(scheduledOn);
    }
}