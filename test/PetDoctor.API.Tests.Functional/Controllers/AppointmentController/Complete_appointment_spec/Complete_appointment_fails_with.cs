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

namespace PetDoctor.API.Tests.Functional.Controllers.AppointmentController.Complete_appointment_spec
{
    [Collection(TestCollections.RealDatabaseTests)]
    public class Complete_appointment_fails_with
    {
        private const string EndpointRoute = "api/v1/appointments";

        private readonly TestFixture _testFixture;
        private readonly Fixture _fixture;

        public Complete_appointment_fails_with(TestFixture testFixture)
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
            var request = _fixture.Create<CompleteAppointment>();
            var uri = $"{EndpointRoute}/{id}/complete";

            var response = await client.PutAsJsonAsync(uri, request);

            response.StatusCode.Should().Be(StatusCodes.Status404NotFound);
        }
    }
}
