using AutoFixture;
using FluentAssertions;
using PetDoctor.Domain.Aggregates.Appointments;
using System;
using Xunit;

namespace PetDoctor.Tests.Unit.Domain.Aggregates.Appointments
{
    public class RescheduleAppointmentTests
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
}
