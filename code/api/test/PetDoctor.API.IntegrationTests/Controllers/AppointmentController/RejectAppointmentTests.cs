using AutoFixture;
using FluentAssertions;
using PetDoctor.API.Application.Commands;
using PetDoctor.API.IntegrationTests.Helpers;
using PetDoctor.API.IntegrationTests.Setup;
using PetDoctor.Domain.Aggregates.Appointments;
using System;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using Xunit;

namespace PetDoctor.API.IntegrationTests.Controllers.AppointmentController;

[Collection(TestCollections.RealDatabaseTests)]
public class RejectAppointmentTests
{
    private const string EndpointRoute = "v1/appointments";

    private readonly TestFixture _testFixture;
    private readonly Fixture _fixture;

    public RejectAppointmentTests(TestFixture testFixture)
    {
        _testFixture = testFixture;
        _fixture = new Fixture();
        _fixture.Customize(new CreateAppointmentCustomization());
    }

    [Fact]
    [ResetDatabase]
    public async Task Successful_requests_return_204_no_content()
    {
        var client = _testFixture.Client;
        var seeder = new AppointmentSeeder();
        var id = await seeder.CreateAppointment(client);
        var request = _fixture.Create<RejectAppointment>();
        var uri = $"{EndpointRoute}/{id}/reject";

        var response = await client.PutAsJsonAsync(uri, request);
        await response.ThrowWithBodyIfUnsuccessfulStatusCode();

        response.StatusCode.Should().Be(HttpStatusCode.NoContent);
    }

    [Fact]
    [ResetDatabase]
    public async Task Rejected_appointments_are_persisted_correctly()
    {
        var client = _testFixture.Client;
        var seeder = new AppointmentSeeder();
        var id = await seeder.CreateAppointment(client);
        var request = _fixture.Create<RejectAppointment>();
        var uri = $"{EndpointRoute}/{id}/reject";

        var response = await client.PutAsJsonAsync(uri, request);
        await response.ThrowWithBodyIfUnsuccessfulStatusCode();

        var sut = await _testFixture.FindAppointment(id);
        sut.State.Should().Be(AppointmentState.Rejected);
    }

    [Fact]
    [ResetDatabase]
    public async Task Rejecting_an_appointment_captures_the_rejection_reason_correctly()
    {
        var client = _testFixture.Client;
        var seeder = new AppointmentSeeder();
        var id = await seeder.CreateAppointment(client);
        var request = _fixture.Create<RejectAppointment>();
        var uri = $"{EndpointRoute}/{id}/reject";

        var response = await client.PutAsJsonAsync(uri, request);
        await response.ThrowWithBodyIfUnsuccessfulStatusCode();

        var sut = await _testFixture.FindAppointment(id);
        sut.RejectionReason.Should().Be(request.Reason);
    }

    [Fact]
    [ResetDatabase]
    public async Task Rejecting_an_appointment_fails_with_a_404_not_found_when_the_appointment_does_not_exist()
    {
        var client = _testFixture.Client;
        var id = Guid.NewGuid();
        var request = _fixture.Create<RejectAppointment>();
        var uri = $"{EndpointRoute}/{id}/reject";

        var response = await client.PutAsJsonAsync(uri, request);

        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }
}
