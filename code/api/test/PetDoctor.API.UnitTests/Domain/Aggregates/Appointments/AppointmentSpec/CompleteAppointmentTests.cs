using AutoFixture;
using FluentAssertions;
using PetDoctor.Domain.Aggregates.Appointments;
using Xunit;

namespace PetDoctor.API.UnitTests.Domain.Aggregates.Appointments.AppointmentSpec;

public class CompleteTests
{
    [Fact]
    public void should_update_state_to_completed()
    {
        var fixture = new Fixture();
        var sut = fixture.Create<Appointment>();

        sut.Complete();

        sut.State.Should().Be(AppointmentState.Completed);
    }
}