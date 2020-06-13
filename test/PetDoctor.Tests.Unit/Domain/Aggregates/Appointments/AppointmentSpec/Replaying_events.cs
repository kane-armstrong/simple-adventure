using System;
using AutoFixture;
using FluentAssertions;
using PetDoctor.Domain;
using PetDoctor.Domain.Aggregates.Appointments;
using PetDoctor.Domain.Aggregates.Appointments.Events;
using System.Collections.Generic;
using Xunit;

namespace PetDoctor.Tests.Unit.Domain.Aggregates.Appointments.AppointmentSpec
{
    public class Replaying_events
    {
        [Fact]
        public void does_not_affect_the_appointment_when_events_are_empty()
        {
            var fixture = new Fixture();
            var createdEvent = fixture.Create<AppointmentCreated>();

            var sut = new Appointment(createdEvent);
            sut.ReplayEvents(new List<DomainEvent>());

            var other = new Appointment(createdEvent);

            sut.Should().BeEquivalentTo(other);
        }

        [Fact]
        public void produces_the_correct_results_given_a_non_empty_set()
        {
            var fixture = new Fixture();
            var createdEvent = fixture.Create<AppointmentCreated>();

            var sut = new Appointment(createdEvent);

            var vetId = Guid.NewGuid();
            var newDate = sut.ScheduledOn.AddDays(3);

            var events = new List<DomainEvent>
            {
                new AppointmentConfirmed(sut.Id, vetId),
                new AppointmentRescheduled(sut.Id, newDate),
                new AppointmentConfirmed(sut.Id, vetId),
                new AppointmentMembersCheckedIn(sut.Id),
                new AppointmentCompleted(sut.Id)
            };

            sut.ReplayEvents(events);

            sut.State.Should().Be(AppointmentState.Completed);
            sut.ScheduledOn.Should().Be(newDate);
            sut.AttendingVeterinarianId.Should().Be(vetId);
        }
    }
}
