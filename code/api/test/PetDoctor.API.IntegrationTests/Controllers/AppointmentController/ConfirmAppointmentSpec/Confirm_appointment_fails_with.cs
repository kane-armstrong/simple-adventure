using AutoFixture;
using FluentAssertions;
using PetDoctor.API.Application.Commands;
using PetDoctor.API.IntegrationTests.Helpers;
using PetDoctor.API.IntegrationTests.Setup;
using System;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using Xunit;

namespace PetDoctor.API.IntegrationTests.Controllers.AppointmentController.ConfirmAppointmentSpec
{
    [Collection(TestCollections.RealDatabaseTests)]
    public class Confirm_appointment_fails_with
    {
        private const string EndpointRoute = "v1/appointments";

        private readonly TestFixture _testFixture;
        private readonly Fixture _fixture;

        public Confirm_appointment_fails_with(TestFixture testFixture)
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
            var request = _fixture.Create<ConfirmAppointment>();
            var uri = $"{EndpointRoute}/{id}/confirm";

            var response = await client.PutAsJsonAsync(uri, request);

            response.StatusCode.Should().Be(HttpStatusCode.NotFound);
        }
    }
}
