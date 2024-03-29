﻿using AutoFixture;
using FluentAssertions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using PetDoctor.API.Application.Commands;
using PetDoctor.API.IntegrationTests.Helpers;
using PetDoctor.API.IntegrationTests.Setup;
using PetDoctor.Domain.Aggregates.Appointments;
using System.Net;
using Xunit;

namespace PetDoctor.API.IntegrationTests.Controllers.AppointmentController;

[Collection(TestCollections.RealDatabaseTests)]
public class CompleteAppointmentTests
{
    private const string EndpointRoute = "v1/appointments";

    private readonly TestFixture _testFixture;
    private readonly Fixture _fixture;

    public CompleteAppointmentTests(TestFixture testFixture)
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
        var id = await AppointmentSeeder.CreateAppointment(client);
        var request = _fixture.Create<CompleteAppointment>();
        var uri = $"{EndpointRoute}/{id}/complete";

        var response = await client.PutAsJsonAsync(uri, request);
        await response.ThrowWithBodyIfUnsuccessfulStatusCode();

        response.StatusCode.Should().Be(HttpStatusCode.NoContent);
    }

    [Fact]
    [ResetDatabase]
    public async Task A_completed_appointment_is_persisted_correctly()
    {
        var client = _testFixture.Client;
        var id = await AppointmentSeeder.CreateAppointment(client);
        var request = _fixture.Create<CompleteAppointment>();
        var uri = $"{EndpointRoute}/{id}/complete";

        var response = await client.PutAsJsonAsync(uri, request);
        await response.ThrowWithBodyIfUnsuccessfulStatusCode();

        var sut = await _testFixture.FindAppointment(id);
        sut?.State.Should().Be(AppointmentState.Completed);
    }

    [Fact]
    [ResetDatabase]
    public async Task Completing_an_appointment_fails_with_404_not_found_when_the_appointment_does_not_exist()
    {
        var client = _testFixture.Client;
        var id = Guid.NewGuid();
        var request = _fixture.Create<CompleteAppointment>();
        var uri = $"{EndpointRoute}/{id}/complete";

        var response = await client.PutAsJsonAsync(uri, request);

        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    [Fact]
    [ResetDatabase]
    public async Task Completing_an_appointment_fails_with_the_correct_response_body_when_the_appointment_does_not_exist()
    {
        var client = _testFixture.Client;
        var id = Guid.NewGuid();
        var request = _fixture.Create<CompleteAppointment>();
        var uri = $"{EndpointRoute}/{id}/complete";

        var response = await client.PutAsJsonAsync(uri, request);

        var body = await response.Content.ReadAsStringAsync();
        var payload = JsonConvert.DeserializeObject<ProblemDetails>(body);
        payload.Should().BeEquivalentTo(new
        {
            Detail = "The requested resource was not found",
            Status = StatusCodes.Status404NotFound,
            Title = "Not Found"
        });
        Uri.IsWellFormedUriString(payload!.Instance, UriKind.Absolute).Should().BeTrue();
        Uri.IsWellFormedUriString(payload.Type, UriKind.Absolute).Should().BeTrue();
    }
}