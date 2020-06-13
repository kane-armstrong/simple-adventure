using AutoFixture;
using FluentAssertions;
using Microsoft.AspNetCore.Http;
using PetDoctor.API.Application.Commands;
using PetDoctor.API.Tests.Functional.Setup;
using System;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using Xunit;

namespace PetDoctor.API.Tests.Functional.Controllers.AppointmentController.CreateAppointmentSpec
{
    public class A_successful_create_appointment_request : IClassFixture<TestFixture>
    {
        private const string EndpointRoute = "api/v1/appointments";

        private readonly TestFixture _testFixture;

        public A_successful_create_appointment_request(TestFixture testFixture)
        {
            _testFixture = testFixture;
        }

        [Fact]
        public async Task returns_201_created()
        {
            var client = _testFixture.Client;
            var fixture = new Fixture();
            var request = fixture.Create<CreateAppointment>();

            var result = await client.PostAsJsonAsync(EndpointRoute, request);

            result.StatusCode.Should().Be(StatusCodes.Status201Created);
        }

        [Fact]
        public async Task returns_the_id_of_created_appointment_in_location_header()
        {
            var client = _testFixture.Client;
            var fixture = new Fixture();
            var request = fixture.Create<CreateAppointment>();

            var response = await client.PostAsJsonAsync(EndpointRoute, request);

            response.IsSuccessStatusCode.Should().BeTrue();
            var expectedLocationPattern = $"{client.BaseAddress}{EndpointRoute}/\\d";
            response.Headers.Location.AbsoluteUri.Should().MatchRegex(expectedLocationPattern);
        }

        [Fact]
        public async Task actually_persists_the_appointment()
        {
            var client = _testFixture.Client;
            var fixture = new Fixture();
            var request = fixture.Create<CreateAppointment>();

            var result = await client.PostAsJsonAsync(EndpointRoute, request);
            var foundIdInLocationHeader = Guid.TryParse(result.Headers.Location.AbsoluteUri.Split('/').Last(), out var id);
            foundIdInLocationHeader.Should().BeTrue();

            var created = await _testFixture.FindAppointment(id);
            created.Should().NotBeNull();
        }
    }
}
