using AutoFixture;
using FluentAssertions;
using PetDoctor.Domain.Aggregates.Appointments;
using System;
using Xunit;

namespace PetDoctor.Tests.Unit.Domain.Aggregates.Appointments.AppointmentSpec
{
    public class Confirming_an_appointment
    {
        [Fact]
        public void should_update_state_to_confirmed()
        {
            var fixture = new Fixture();
            var sut = fixture.Create<Appointment>();

            sut.Confirm(Guid.NewGuid());

            sut.State.Should().Be(AppointmentState.Confirmed);
        }

        [Fact]
        public void should_set_vet_id_correctly()
        {
            var fixture = new Fixture();
            var sut = fixture.Create<Appointment>();

            var id = Guid.NewGuid();
            sut.Confirm(id);

            sut.AttendingVeterinarianId.Should().Be(id);
        }
    }
}
