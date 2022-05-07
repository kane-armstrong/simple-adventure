using AutoFixture;
using FluentAssertions;
using PetDoctor.Domain;
using PetDoctor.Domain.Aggregates.Appointments;
using PetDoctor.Domain.Aggregates.Appointments.Events;
using Xunit;

namespace PetDoctor.API.UnitTests.Domain.Aggregates.Appointments.AppointmentSpec;

public class ReplayEventsTests
{
    [Fact]
    public void Applying_an_appointment_canceled_event_should_update_state_to_canceled()
    {
        var fixture = new Fixture();
        var sut = fixture.Create<Appointment>();

        var @event = new AppointmentCanceled(sut.Id, "i went somewhere else");

        sut.ReplayEvents(new List<DomainEvent> { @event });

        sut.State.Should().Be(AppointmentState.Canceled);
    }

    [Fact]
    public void Applying_an_appointment_canceled_event_should_set_cancellation_reason_correctly()
    {
        var fixture = new Fixture();
        var sut = fixture.Create<Appointment>();

        var @event = new AppointmentCanceled(sut.Id, "i went somewhere else");

        sut.ReplayEvents(new List<DomainEvent> { @event });

        sut.CancellationReason.Should().Be(@event.CancellationReason);
    }

    [Fact]
    public void Applying_an_appointment_completed_event_should_update_state_to_completed()
    {
        var fixture = new Fixture();
        var sut = fixture.Create<Appointment>();

        var @event = new AppointmentCompleted(sut.Id);

        sut.ReplayEvents(new List<DomainEvent> { @event });

        sut.State.Should().Be(AppointmentState.Completed);
    }

    [Fact]
    public void Applying_an_appointment_checked_in_event_should_update_state_to_checkedin()
    {
        var fixture = new Fixture();
        var sut = fixture.Create<Appointment>();

        var @event = new AppointmentMembersCheckedIn(sut.Id);

        sut.ReplayEvents(new List<DomainEvent> { @event });

        sut.State.Should().Be(AppointmentState.CheckedIn);
    }

    [Fact]
    public void Applying_an_appointment_confirmed_event_should_update_state_to_confirmed()
    {
        var fixture = new Fixture();
        var sut = fixture.Create<Appointment>();

        var @event = new AppointmentConfirmed(sut.Id, Guid.NewGuid());

        sut.ReplayEvents(new List<DomainEvent> { @event });

        sut.State.Should().Be(AppointmentState.Confirmed);
    }

    [Fact]
    public void Applying_an_appointment_confirmed_event_should_set_vet_id_correctly()
    {
        var fixture = new Fixture();
        var sut = fixture.Create<Appointment>();

        var @event = new AppointmentConfirmed(sut.Id, Guid.NewGuid());

        sut.ReplayEvents(new List<DomainEvent> { @event });

        sut.State.Should().Be(AppointmentState.Confirmed);

        sut.AttendingVeterinarianId.Should().Be(@event.AttendingVeterinarianId);
    }

    [Fact]
    public void Applying_an_appointment_rejected_event_should_update_state_to_rejected()
    {
        var fixture = new Fixture();
        var sut = fixture.Create<Appointment>();

        var @event = new AppointmentRejected(sut.Id, "nobody available");

        sut.ReplayEvents(new List<DomainEvent> { @event });

        sut.State.Should().Be(AppointmentState.Rejected);
    }

    [Fact]
    public void Applying_an_appointment_rejected_event_should_set_rejection_reason_correctly()
    {
        var fixture = new Fixture();
        var sut = fixture.Create<Appointment>();

        var @event = new AppointmentRejected(sut.Id, "nobody available");

        sut.ReplayEvents(new List<DomainEvent> { @event });

        sut.RejectionReason.Should().Be(@event.RejectionReason);
    }

    [Fact]
    public void Applying_an_appointment_rescheduled_event_should_update_state_to_requested()
    {
        var fixture = new Fixture();
        var sut = fixture.Create<Appointment>();

        var @event = new AppointmentRescheduled(sut.Id, sut.ScheduledOn.AddDays(2));

        sut.ReplayEvents(new List<DomainEvent> { @event });

        sut.State.Should().Be(AppointmentState.Requested);
    }

    [Fact]
    public void Applying_an_appointment_rescheduled_event_should_set_scheduled_on_correctly()
    {
        var fixture = new Fixture();
        var sut = fixture.Create<Appointment>();

        var @event = new AppointmentRescheduled(sut.Id, sut.ScheduledOn.AddDays(2));

        sut.ReplayEvents(new List<DomainEvent> { @event });

        sut.ScheduledOn.Should().Be(@event.Date);
    }

    [Fact]
    public void Replaying_events_does_not_affect_the_appointment_when_events_are_empty()
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
    public void Replaying_events_produces_the_correct_results_given_a_non_empty_set(
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
        yield return CreateCompletedAppointment();
        yield return CreateCompletedRescheduledAppointment();
        yield return CreateRejectedAppointment();
        yield return CreateRejectedRescheduledAppointment();
        yield return CreateCanceledAppointment();
    }

    private static object[] CreateCompletedAppointment()
    {
        var fixture = new Fixture();
        var createdEvent = fixture.Create<AppointmentCreated>();

        var appointment = new Appointment(createdEvent);

        var vetId = Guid.NewGuid();

        var events = new List<DomainEvent>
        {
            new AppointmentConfirmed(appointment.Id, vetId),
            new AppointmentMembersCheckedIn(appointment.Id),
            new AppointmentCompleted(appointment.Id)
        };

        appointment.Confirm(vetId);
        appointment.CheckIn();
        appointment.Complete();

        return new object[]
        {
            createdEvent, events, appointment
        };
    }

    private static object[] CreateCompletedRescheduledAppointment()
    {
        var fixture = new Fixture();
        var createdEvent = fixture.Create<AppointmentCreated>();

        var appointment = new Appointment(createdEvent);

        var firstVetId = Guid.NewGuid();
        var secondVetId = Guid.NewGuid();
        var newDate = appointment.ScheduledOn.AddDays(3);

        var events = new List<DomainEvent>
        {
            new AppointmentConfirmed(appointment.Id, firstVetId),
            new AppointmentRescheduled(appointment.Id, newDate),
            new AppointmentConfirmed(appointment.Id, secondVetId),
            new AppointmentMembersCheckedIn(appointment.Id),
            new AppointmentCompleted(appointment.Id)
        };

        appointment.Confirm(firstVetId);
        appointment.Reschedule(newDate);
        appointment.Confirm(secondVetId);
        appointment.CheckIn();
        appointment.Complete();

        return new object[]
        {
            createdEvent, events, appointment
        };
    }

    private static object[] CreateRejectedAppointment()
    {
        var fixture = new Fixture();
        var createdEvent = fixture.Create<AppointmentCreated>();

        var appointment = new Appointment(createdEvent);

        var vetId = Guid.NewGuid();
        const string reason = "no";

        var events = new List<DomainEvent>
        {
            new AppointmentConfirmed(appointment.Id, vetId),
            new AppointmentRejected(appointment.Id, reason)
        };

        appointment.Confirm(vetId);
        appointment.Reject(reason);

        return new object[]
        {
            createdEvent, events, appointment
        };
    }

    private static object[] CreateRejectedRescheduledAppointment()
    {
        var fixture = new Fixture();
        var createdEvent = fixture.Create<AppointmentCreated>();

        var appointment = new Appointment(createdEvent);

        var vetId = Guid.NewGuid();
        var newDate = appointment.ScheduledOn.AddDays(3);
        const string reason = "no";

        var events = new List<DomainEvent>
        {
            new AppointmentConfirmed(appointment.Id, vetId),
            new AppointmentRescheduled(appointment.Id, newDate),
            new AppointmentRejected(appointment.Id, reason)
        };

        appointment.Confirm(vetId);
        appointment.Reschedule(newDate);
        appointment.Reject(reason);

        return new object[]
        {
            createdEvent, events, appointment
        };
    }

    private static object[] CreateCanceledAppointment()
    {
        var fixture = new Fixture();
        var createdEvent = fixture.Create<AppointmentCreated>();

        var appointment = new Appointment(createdEvent);

        var vetId = Guid.NewGuid();
        var reason = "no money";

        var events = new List<DomainEvent>
        {
            new AppointmentConfirmed(appointment.Id, vetId),
            new AppointmentCanceled(appointment.Id, reason)
        };

        appointment.Confirm(vetId);
        appointment.Cancel(reason);

        return new object[]
        {
            createdEvent, events, appointment
        };
    }
}