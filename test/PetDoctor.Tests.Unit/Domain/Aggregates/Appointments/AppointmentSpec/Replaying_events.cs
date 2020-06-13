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

        [Theory]
        [MemberData(nameof(TestEvents))]
        public void produces_the_correct_results_given_a_non_empty_set(
            AppointmentCreated createdEvent, 
            List<DomainEvent> events, 
            Appointment expectedState)
        {
            var sut = new Appointment(createdEvent);
            sut.ReplayEvents(events);
            sut.Should().BeEquivalentTo(expectedState, c => c.Excluding(p => p.PendingEvents));
        }

        public static IEnumerable<object[]> TestEvents()
        {
            var fixture = new Fixture();
            var createdEvent = fixture.Create<AppointmentCreated>();

            var appointment = new Appointment(createdEvent);

            var vetId = Guid.NewGuid();
            var newDate = appointment.ScheduledOn.AddDays(3);

            var events = new List<DomainEvent>
            {
                new AppointmentConfirmed(appointment.Id, Guid.NewGuid()),
                new AppointmentRescheduled(appointment.Id, newDate),
                new AppointmentConfirmed(appointment.Id, vetId),
                new AppointmentMembersCheckedIn(appointment.Id),
                new AppointmentCompleted(appointment.Id)
            };

            appointment.Confirm(vetId);
            appointment.Reschedule(newDate);
            appointment.Confirm(vetId);
            appointment.CheckIn();
            appointment.Complete();

            yield return new object[]
            {
                createdEvent, events, appointment
            };
        }
    }
}
