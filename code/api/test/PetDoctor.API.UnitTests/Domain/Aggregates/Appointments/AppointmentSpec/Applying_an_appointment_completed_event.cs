using System.Collections.Generic;
using AutoFixture;
using FluentAssertions;
using PetDoctor.Domain;
using PetDoctor.Domain.Aggregates.Appointments;
using PetDoctor.Domain.Aggregates.Appointments.Events;
using Xunit;

namespace PetDoctor.API.UnitTests.Domain.Aggregates.Appointments.AppointmentSpec;

public class Applying_an_appointment_completed_event
{
    [Fact]
    public void should_update_state_to_completed()
    {
        var fixture = new Fixture();
        var sut = fixture.Create<Appointment>();

        var @event = new AppointmentCompleted(sut.Id);

        sut.ReplayEvents(new List<DomainEvent> { @event });

        sut.State.Should().Be(AppointmentState.Completed);
    }
}