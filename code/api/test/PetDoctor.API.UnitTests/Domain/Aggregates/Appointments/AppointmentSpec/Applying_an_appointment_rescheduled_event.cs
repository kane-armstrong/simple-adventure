using System.Collections.Generic;
using AutoFixture;
using FluentAssertions;
using PetDoctor.Domain;
using PetDoctor.Domain.Aggregates.Appointments;
using PetDoctor.Domain.Aggregates.Appointments.Events;
using Xunit;

namespace PetDoctor.API.UnitTests.Domain.Aggregates.Appointments.AppointmentSpec;

public class Applying_an_appointment_rescheduled_event
{
    [Fact]
    public void should_update_state_to_requested()
    {
        var fixture = new Fixture();
        var sut = fixture.Create<Appointment>();

        var @event = new AppointmentRescheduled(sut.Id, sut.ScheduledOn.AddDays(2));

        sut.ReplayEvents(new List<DomainEvent> { @event });

        sut.State.Should().Be(AppointmentState.Requested);
    }

    [Fact]
    public void should_set_scheduled_on_correctly()
    {
        var fixture = new Fixture();
        var sut = fixture.Create<Appointment>();

        var @event = new AppointmentRescheduled(sut.Id, sut.ScheduledOn.AddDays(2));

        sut.ReplayEvents(new List<DomainEvent> { @event });

        sut.ScheduledOn.Should().Be(@event.Date);
    }
}