using System;
using AutoFixture;
using FluentAssertions;
using PetDoctor.Domain.Aggregates.Appointments;
using Xunit;

namespace PetDoctor.API.UnitTests.Domain.Aggregates.Appointments.AppointmentSpec;

public class CreateTests
{
    [Fact]
    public void Creating_an_appointment_should_set_pet_correctly()
    {
        var fixture = new Fixture();
        var pet = fixture.Create<Pet>();
        var owner = fixture.Create<Owner>();

        var sut = new Appointment(pet, owner, "reason", DateTimeOffset.UtcNow);

        sut.Pet.Should().BeEquivalentTo(pet);
    }

    [Fact]
    public void Creating_an_appointment_should_set_owner_correctly()
    {
        var fixture = new Fixture();
        var pet = fixture.Create<Pet>();
        var owner = fixture.Create<Owner>();

        var sut = new Appointment(pet, owner, "reason", DateTimeOffset.UtcNow);

        sut.Owner.Should().BeEquivalentTo(owner);
    }

    [Fact]
    public void Creating_an_appointment_should_set_reason_correctly()
    {
        var fixture = new Fixture();
        var pet = fixture.Create<Pet>();
        var owner = fixture.Create<Owner>();

        const string reason = "time to get vaccinated";

        var sut = new Appointment(pet, owner, reason, DateTimeOffset.UtcNow);

        sut.ReasonForVisit.Should().Be(reason);
    }

    [Fact]
    public void Creating_an_appointment_should_set_scheduled_on_correctly()
    {
        var fixture = new Fixture();
        var pet = fixture.Create<Pet>();
        var owner = fixture.Create<Owner>();

        var scheduledOn = DateTimeOffset.Now.AddDays(5);

        var sut = new Appointment(pet, owner, "reason", scheduledOn);

        sut.ScheduledOn.Should().Be(scheduledOn);
    }

    [Fact]
    public void Creating_an_appointment_should_set_vet_id_correctly()
    {
        var fixture = new Fixture();
        var pet = fixture.Create<Pet>();
        var owner = fixture.Create<Owner>();

        var vetId = Guid.NewGuid();

        var sut = new Appointment(pet, owner, vetId, "reason", DateTimeOffset.Now);

        sut.AttendingVeterinarianId.Should().Be(vetId);
    }

    [Fact]
    public void Creating_an_appointment_should_set_state_to_requested()
    {
        var fixture = new Fixture();
        var pet = fixture.Create<Pet>();
        var owner = fixture.Create<Owner>();

        var vetId = Guid.NewGuid();

        var sut = new Appointment(pet, owner, vetId, "reason", DateTimeOffset.Now);

        sut.State.Should().Be(AppointmentState.Requested);
    }
}