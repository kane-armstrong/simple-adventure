using AutoFixture;
using FluentAssertions;
using PetDoctor.Domain.Aggregates.Appointments;
using PetDoctor.Domain.Aggregates.Appointments.Events;
using Xunit;

namespace PetDoctor.Tests.Unit.Domain.Aggregates.Appointments
{
    public class AppointmentConstructionFromEventTests
    {
        [Fact]
        public void Rehydrating_an_appointment_should_set_pet_correctly()
        {
            var fixture = new Fixture();

            var @event = fixture.Create<AppointmentCreated>();

            var sut = new Appointment(@event);

            sut.Pet.Should().BeEquivalentTo(@event.Data.Pet);
        }

        [Fact]
        public void Rehydrating_an_appointment_should_set_owner_correctly()
        {
            var fixture = new Fixture();

            var @event = fixture.Create<AppointmentCreated>();

            var sut = new Appointment(@event);

            sut.Owner.Should().BeEquivalentTo(@event.Data.Owner);
        }

        [Fact]
        public void Rehydrating_an_appointment_should_set_reason_correctly()
        {
            var fixture = new Fixture();

            var @event = fixture.Create<AppointmentCreated>();

            var sut = new Appointment(@event);

            sut.ReasonForVisit.Should().Be(@event.Data.ReasonForVisit);
        }

        [Fact]
        public void Rehydrating_an_appointment_should_set_scheduled_on_correctly()
        {
            var fixture = new Fixture();

            var @event = fixture.Create<AppointmentCreated>();

            var sut = new Appointment(@event);

            sut.ScheduledOn.Should().Be(@event.Data.ScheduledOn);
        }

        [Fact]
        public void Rehydrating_an_appointment_should_set_vet_id_correctly()
        {
            var fixture = new Fixture();

            var @event = fixture.Create<AppointmentCreated>();

            var sut = new Appointment(@event);

            sut.AttendingVeterinarianId.Should().Be(@event.Data.AttendingVeterinarianId);
        }

        [Fact]
        public void Rehydrating_an_appointment_should_set_state_to_requested()
        {
            var fixture = new Fixture();
            var sut = fixture.Create<Appointment>();

            sut.State.Should().Be(AppointmentState.Requested);
        }
    }
}
