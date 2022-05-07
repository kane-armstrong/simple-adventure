using AutoFixture;
using FluentAssertions;
using PetDoctor.Domain.Aggregates.Appointments;
using Xunit;

namespace PetDoctor.API.UnitTests.Domain.Aggregates.Appointments.AppointmentSpec;

public class ConfirmTests
{
    [Fact]
    public void Confirming_an_appointment_should_update_state_to_confirmed()
    {
        var fixture = new Fixture();
        var sut = fixture.Create<Appointment>();

        sut.Confirm(Guid.NewGuid());

        sut.State.Should().Be(AppointmentState.Confirmed);
    }

    [Fact]
    public void Confirming_an_appointment_should_set_vet_id_correctly()
    {
        var fixture = new Fixture();
        var sut = fixture.Create<Appointment>();

        var id = Guid.NewGuid();
        sut.Confirm(id);

        sut.AttendingVeterinarianId.Should().Be(id);
    }

    [Fact]
    public void Confirming_an_appointment_should_set_vet_id_correctly_when_reconfirming()
    {
        var fixture = new Fixture();
        var sut = fixture.Create<Appointment>();

        var firstVet = Guid.NewGuid();
        sut.Confirm(firstVet);

        var secondVet = Guid.NewGuid();
        sut.Confirm(secondVet);

        sut.AttendingVeterinarianId.Should().Be(secondVet);
    }
}