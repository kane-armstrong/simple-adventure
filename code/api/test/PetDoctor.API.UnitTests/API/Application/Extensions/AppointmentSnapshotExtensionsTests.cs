using AutoFixture;
using FluentAssertions;
using PetDoctor.API.Application.Extensions;
using PetDoctor.Domain.Aggregates.Appointments;
using Xunit;

namespace PetDoctor.API.UnitTests.API.Application.Extensions;

public class AppointmentSnapshotExtensionsTests
{
    [Fact]
    public void Mapping_an_appointment_snapshot_sets_id_correctly()
    {
        var fixture = new Fixture();

        var snapshot = fixture.Create<AppointmentSnapshot>();

        var sut = snapshot.ToAppointmentView();

        sut.Id.Should().Be(snapshot.Id);
    }

    [Fact]
    public void Mapping_an_appointment_snapshot_sets_pet_correctly()
    {
        var fixture = new Fixture();

        var snapshot = fixture.Create<AppointmentSnapshot>();

        var sut = snapshot.ToAppointmentView();

        sut.Pet.Should().BeEquivalentTo(snapshot.Pet);
    }

    [Fact]
    public void Mapping_an_appointment_snapshot_sets_owner_correctly()
    {
        var fixture = new Fixture();

        var snapshot = fixture.Create<AppointmentSnapshot>();

        var sut = snapshot.ToAppointmentView();

        sut.Owner.Should().BeEquivalentTo(snapshot.Owner);
    }

    [Fact]
    public void Mapping_an_appointment_snapshot_sets_vet_id_correctly()
    {
        var fixture = new Fixture();

        var snapshot = fixture.Create<AppointmentSnapshot>();

        var sut = snapshot.ToAppointmentView();

        sut.AttendingVeterinarianId.Should().Be(snapshot.AttendingVeterinarianId);
    }

    [Fact]
    public void Mapping_an_appointment_snapshot_sets_reason_for_visit_correctly()
    {
        var fixture = new Fixture();

        var snapshot = fixture.Create<AppointmentSnapshot>();

        var sut = snapshot.ToAppointmentView();

        sut.ReasonForVisit.Should().Be(snapshot.ReasonForVisit);
    }

    [Fact]
    public void Mapping_an_appointment_snapshot_sets_rejection_reason_correctly()
    {
        var fixture = new Fixture();

        var snapshot = fixture.Create<AppointmentSnapshot>();

        var sut = snapshot.ToAppointmentView();

        sut.RejectionReason.Should().Be(snapshot.RejectionReason);
    }

    [Fact]
    public void Mapping_an_appointment_snapshot_sets_cancellation_reason_correctly()
    {
        var fixture = new Fixture();

        var snapshot = fixture.Create<AppointmentSnapshot>();

        var sut = snapshot.ToAppointmentView();

        sut.CancellationReason.Should().Be(snapshot.CancellationReason);
    }

    [Fact]
    public void Mapping_an_appointment_snapshot_sets_scheduled_on_correctly()
    {
        var fixture = new Fixture();

        var snapshot = fixture.Create<AppointmentSnapshot>();

        var sut = snapshot.ToAppointmentView();

        sut.ScheduledOn.Should().Be(snapshot.ScheduledOn);
    }

    [Fact]
    public void Mapping_an_appointment_snapshot_sets_state_correctly()
    {
        var fixture = new Fixture();

        var snapshot = fixture.Create<AppointmentSnapshot>();

        var sut = snapshot.ToAppointmentView();

        sut.State.Should().Be(snapshot.State.ToString());
    }
}