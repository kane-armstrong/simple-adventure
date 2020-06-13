using AutoFixture;
using FluentAssertions;
using PetDoctor.Domain.Aggregates.Appointments;
using Xunit;

namespace PetDoctor.Tests.Unit.Domain.Aggregates.Appointments
{
    public class RejectAppointmentTests
    {
        [Fact]
        public void Rejecting_an_appointment_should_update_state_to_rejected()
        {
            var fixture = new Fixture();
            var sut = fixture.Create<Appointment>();

            sut.Reject("nobody available at the requested time");

            sut.State.Should().Be(AppointmentState.Rejected);
        }

        [Fact]
        public void Rejecting_an_appointment_should_set_rejection_reason_correctly()
        {
            var fixture = new Fixture();
            var sut = fixture.Create<Appointment>();

            const string reason = "sorry but there is nobody available";

            sut.Reject(reason);

            sut.RejectionReason.Should().Be(reason);
        }
    }
}
