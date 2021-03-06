﻿using AutoFixture;
using FluentAssertions;
using PetDoctor.API.Application.Extensions;
using PetDoctor.Domain.Aggregates.Appointments;
using Xunit;

namespace PetDoctor.Tests.Unit.API.Application.Extensions.AppointmentExtensionsSpec
{
    public class Mapping_an_appointment
    {
        [Fact]
        public void set_id_correctly()
        {
            var fixture = new Fixture();

            var snapshot = fixture.Create<Appointment>();

            var sut = snapshot.ToAppointmentView();

            sut.Id.Should().Be(snapshot.Id);
        }

        [Fact]
        public void set_pet_correctly()
        {
            var fixture = new Fixture();

            var snapshot = fixture.Create<Appointment>();

            var sut = snapshot.ToAppointmentView();

            sut.Pet.Should().BeEquivalentTo(snapshot.Pet);
        }

        [Fact]
        public void set_owner_correctly()
        {
            var fixture = new Fixture();

            var snapshot = fixture.Create<Appointment>();

            var sut = snapshot.ToAppointmentView();

            sut.Owner.Should().BeEquivalentTo(snapshot.Owner);
        }

        [Fact]
        public void set_vet_id_correctly()
        {
            var fixture = new Fixture();

            var snapshot = fixture.Create<Appointment>();

            var sut = snapshot.ToAppointmentView();

            sut.AttendingVeterinarianId.Should().Be(snapshot.AttendingVeterinarianId);
        }

        [Fact]
        public void set_reason_for_visit_correctly()
        {
            var fixture = new Fixture();

            var snapshot = fixture.Create<Appointment>();

            var sut = snapshot.ToAppointmentView();

            sut.ReasonForVisit.Should().Be(snapshot.ReasonForVisit);
        }

        [Fact]
        public void set_rejection_reason_correctly()
        {
            var fixture = new Fixture();

            var snapshot = fixture.Create<Appointment>();

            var sut = snapshot.ToAppointmentView();

            sut.RejectionReason.Should().Be(snapshot.RejectionReason);
        }

        [Fact]
        public void set_cancellation_reason_correctly()
        {
            var fixture = new Fixture();

            var snapshot = fixture.Create<Appointment>();

            var sut = snapshot.ToAppointmentView();

            sut.CancellationReason.Should().Be(snapshot.CancellationReason);
        }

        [Fact]
        public void set_scheduled_on_correctly()
        {
            var fixture = new Fixture();

            var snapshot = fixture.Create<Appointment>();

            var sut = snapshot.ToAppointmentView();

            sut.ScheduledOn.Should().Be(snapshot.ScheduledOn);
        }

        [Fact]
        public void set_state_correctly()
        {
            var fixture = new Fixture();

            var snapshot = fixture.Create<Appointment>();

            var sut = snapshot.ToAppointmentView();

            sut.State.Should().Be(snapshot.State.ToString());
        }
    }
}
