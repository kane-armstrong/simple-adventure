using AutoFixture;
using FluentAssertions;
using PetDoctor.API.Application.Commands;
using PetDoctor.API.IntegrationTests.Helpers;
using PetDoctor.API.IntegrationTests.Setup;
using System;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using Xunit;

namespace PetDoctor.API.IntegrationTests.Controllers.AppointmentController;

[Collection(TestCollections.RealDatabaseTests)]
public class CreateAppointmentTests
{
    private const string EndpointRoute = "v1/appointments";

    private readonly TestFixture _testFixture;
    private readonly Fixture _fixture;

    public CreateAppointmentTests(TestFixture testFixture)
    {
        _testFixture = testFixture;
        _fixture = new Fixture();
        _fixture.Customize(new CreateAppointmentCustomization());
    }

    [Fact]
    [ResetDatabase]
    public async Task Successful_requests_return_201_created()
    {
        var client = _testFixture.Client;
        var request = _fixture.Create<CreateAppointment>();

        var response = await client.PostAsJsonAsync(EndpointRoute, request);
        await response.ThrowWithBodyIfUnsuccessfulStatusCode();

        response.StatusCode.Should().Be(HttpStatusCode.Created);
    }

    [Fact]
    [ResetDatabase]
    public async Task Successful_requests_include_the_id_of_created_appointment_in_location_header()
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
    public async Task Appointments_are_persisted_correctly()
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