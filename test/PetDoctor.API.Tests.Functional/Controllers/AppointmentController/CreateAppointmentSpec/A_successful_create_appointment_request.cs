using AutoFixture;
using FluentAssertions;
using Microsoft.AspNetCore.Http;
using PetDoctor.API.Application.Commands;
using PetDoctor.API.Tests.Functional.Helpers;
using PetDoctor.API.Tests.Functional.Setup;
using System;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using Xunit;

namespace PetDoctor.API.Tests.Functional.Controllers.AppointmentController.CreateAppointmentSpec
{
    [Collection(TestCollections.RealDatabaseTests)]
    public class A_successful_create_appointment_request
    {
        private const string EndpointRoute = "api/v1/appointments";

        private readonly TestFixture _testFixture;
        private readonly Fixture _fixture;

        public A_successful_create_appointment_request(TestFixture testFixture)
        {
            _testFixture = testFixture;
            _fixture = new Fixture();
            _fixture.Customize(new CreateAppointmentCustomization());
        }

        [Fact]
        [ResetDatabase]
        public async Task returns_201_created()
        {
            var client = _testFixture.Client;
            var request = _fixture.Create<CreateAppointment>();

            var response = await client.PostAsJsonAsync(EndpointRoute, request);
            await response.ThrowWithBodyIfUnsuccessfulStatusCode();

            response.StatusCode.Should().Be(StatusCodes.Status201Created);
        }

        [Fact]
        [ResetDatabase]
        public async Task returns_the_id_of_created_appointment_in_location_header()
        {
            var client = _testFixture.Client;
            var request = _fixture.Create<CreateAppointment>();

            var response = await client.PostAsJsonAsync(EndpointRoute, request);
            await response.ThrowWithBodyIfUnsuccessfulStatusCode();

            const string guidPattern = "[({]?[a-fA-F0-9]{8}[-]?([a-fA-F0-9]{4}[-]?){3}[a-fA-F0-9]{12}[})]?";
            response.Headers.Location.AbsoluteUri.Should().MatchRegex($"{client.BaseAddress}{EndpointRoute}/{guidPattern}");
        }

        [Fact]
        [ResetDatabase]
        public async Task actually_persists_the_appointment()
        {
            var client = _testFixture.Client;

            var request = _fixture.Create<CreateAppointment>();

            var response = await client.PostAsJsonAsync(EndpointRoute, request);
            await response.ThrowWithBodyIfUnsuccessfulStatusCode();

            var foundIdInLocationHeader = Guid.TryParse(response.Headers.Location.AbsoluteUri.Split('/').Last(), out var id);
            foundIdInLocationHeader.Should().BeTrue();

            var created = await _testFixture.FindAppointment(id);
            created.Should().NotBeNull();
        }
    }
}
