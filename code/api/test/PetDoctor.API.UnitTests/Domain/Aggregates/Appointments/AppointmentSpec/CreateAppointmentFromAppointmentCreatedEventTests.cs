using AutoFixture;
using FluentAssertions;
using PetDoctor.Domain.Aggregates.Appointments;
using PetDoctor.Domain.Aggregates.Appointments.Events;
using Xunit;

namespace PetDoctor.API.UnitTests.Domain.Aggregates.Appointments.AppointmentSpec;

public class CreateFromAppointmentCreatedEventTests
{
    [Fact]
    public void Creating_appointment_from_appointmentcreated_event_should_set_id_correctly()
    {
        var fixture = new Fixture();

        var @event = fixture.Create<AppointmentCreated>();

        var sut = new Appointment(@event);

        sut.Id.Should().Be(@event.AppointmentId);
    }

    [Fact]
    public void Creating_appointment_from_appointmentcreated_event_should_set_pet_correctly()
    {
        var fixture = new Fixture();

        var @event = fixture.Create<AppointmentCreated>();

        var sut = new Appointment(@event);

        sut.Pet.Should().BeEquivalentTo(@event.Data.Pet);
    }

    [Fact]
    public void Creating_appointment_from_appointmentcreated_event_should_set_owner_correctly()
    {
        var fixture = new Fixture();

        var @event = fixture.Create<AppointmentCreated>();

        var sut = new Appointment(@event);

        sut.Owner.Should().BeEquivalentTo(@event.Data.Owner);
    }

    [Fact]
    public void Creating_appointment_from_appointmentcreated_event_should_set_reason_correctly()
    {
        var fixture = new Fixture();

        var @event = fixture.Create<AppointmentCreated>();

        var sut = new Appointment(@event);

        sut.ReasonForVisit.Should().Be(@event.Data.ReasonForVisit);
    }

    [Fact]
    public void Creating_appointment_from_appointmentcreated_event_should_set_scheduled_on_correctly()
    {
        var fixture = new Fixture();

        var @event = fixture.Create<AppointmentCreated>();

        var sut = new Appointment(@event);

        sut.ScheduledOn.Should().Be(@event.Data.ScheduledOn);
    }

    [Fact]
    public void Creating_appointment_from_appointmentcreated_event_should_set_vet_id_correctly()
    {
        var fixture = new Fixture();

        var @event = fixture.Create<AppointmentCreated>();

        var sut = new Appointment(@event);

        sut.AttendingVeterinarianId.Should().Be(@event.Data.AttendingVeterinarianId);
    }

    [Fact]
    public void Creating_appointment_from_appointmentcreated_event_should_set_state_to_requested()
    {
        var fixture = new Fixture();
        var pet = fixture.Create<Pet>();
        var owner = fixture.Create<Owner>();

        var apt = new Appointment(pet, owner, "reasons", DateTimeOffset.Now.AddDays(3));

        var @event = apt.PendingEvents.First() as AppointmentCreated;

        var sut = new Appointment(@event!);

        sut.State.Should().Be(AppointmentState.Requested);
    }
}