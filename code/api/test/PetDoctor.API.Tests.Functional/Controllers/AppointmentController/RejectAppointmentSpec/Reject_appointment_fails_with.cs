using AutoFixture;
using FluentAssertions;
using Microsoft.AspNetCore.Http;
using PetDoctor.API.Application.Commands;
using PetDoctor.API.Tests.Functional.Helpers;
using PetDoctor.API.Tests.Functional.Setup;
using System;
using System.Net.Http;
using System.Threading.Tasks;
using Xunit;

namespace PetDoctor.API.Tests.Functional.Controllers.AppointmentController.RejectAppointmentSpec
{
    [Collection(TestCollections.RealDatabaseTests)]
    public class Reject_appointment_fails_with
    {
        private const string EndpointRoute = "v1/appointments";

        private readonly TestFixture _testFixture;
        private readonly Fixture _fixture;

        public Reject_appointment_fails_with(TestFixture testFixture)
        {
            _testFixture = testFixture;
            _fixture = new Fixture();
            _fixture.Customize(new CreateAppointmentCustomization());
        }

        [Fact]
        [ResetDatabase]
        public async Task a_404_not_found_when_the_appointment_does_not_exist()
        {
            var client = _testFixture.Client;
            var id = Guid.NewGuid();
            var request = _fixture.Create<RejectAppointment>();
            var uri = $"{EndpointRoute}/{id}/reject";

            var response = await client.PutAsJsonAsync(uri, request);

            response.StatusCode.Should().Be(StatusCodes.Status404NotFound);
        }
    }
}
