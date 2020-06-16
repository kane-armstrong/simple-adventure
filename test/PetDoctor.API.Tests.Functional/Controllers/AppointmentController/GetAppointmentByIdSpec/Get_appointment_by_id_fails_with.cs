using FluentAssertions;
using Microsoft.AspNetCore.Http;
using PetDoctor.API.Tests.Functional.Setup;
using System;
using System.Threading.Tasks;
using Xunit;

namespace PetDoctor.API.Tests.Functional.Controllers.AppointmentController.GetAppointmentByIdSpec
{
    [Collection(TestCollections.RealDatabaseTests)]
    public class Get_appointment_by_id_fails_with
    {
        private const string EndpointRoute = "v1/appointments";

        private readonly TestFixture _testFixture;

        public Get_appointment_by_id_fails_with(TestFixture testFixture)
        {
            _testFixture = testFixture;
        }

        [Fact]
        [ResetDatabase]
        public async Task a_404_not_found_when_the_appointment_does_not_exist()
        {
            var client = _testFixture.Client;
            var uri = $"{EndpointRoute}/{Guid.NewGuid()}";

            var result = await client.GetAsync(uri);

            result.StatusCode.Should().Be(StatusCodes.Status404NotFound);
        }
    }
}
