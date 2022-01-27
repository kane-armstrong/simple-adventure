﻿using AutoFixture;
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
public class CancelAppointmentTests
{
    private const string EndpointRoute = "v1/appointments";

    private readonly TestFixture _testFixture;
    private readonly Fixture _fixture;

    public CancelAppointmentTests(TestFixture testFixture)
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
        var request = _fixture.Create<CancelAppointment>();
        var uri = $"{EndpointRoute}/{id}/cancel";

        var response = await client.PutAsJsonAsync(uri, request);
        await response.ThrowWithBodyIfUnsuccessfulStatusCode();

        response.StatusCode.Should().Be(HttpStatusCode.NoContent);
    }

    [Fact]
    [ResetDatabase]
    public async Task Appointment_cancellations_are_persisted_correctly()
    {
        var client = _testFixture.Client;
        var seeder = new AppointmentSeeder();
        var id = await seeder.CreateAppointment(client);
        var request = _fixture.Create<CancelAppointment>();
        var uri = $"{EndpointRoute}/{id}/cancel";

        var response = await client.PutAsJsonAsync(uri, request);
        await response.ThrowWithBodyIfUnsuccessfulStatusCode();

        var sut = await _testFixture.FindAppointment(id);
        sut.State.Should().Be(AppointmentState.Canceled);
    }

    [Fact]
    [ResetDatabase]
    public async Task The_reason_for_cancellation_is_captured_correctly()
    {
        var client = _testFixture.Client;
        var seeder = new AppointmentSeeder();
        var id = await seeder.CreateAppointment(client);
        var request = _fixture.Create<CancelAppointment>();
        var uri = $"{EndpointRoute}/{id}/cancel";

        var response = await client.PutAsJsonAsync(uri, request);
        await response.ThrowWithBodyIfUnsuccessfulStatusCode();

        var sut = await _testFixture.FindAppointment(id);
        sut.CancellationReason.Should().Be(request.Reason);
    }

    [Fact]
    [ResetDatabase]
    public async Task Cancelling_an_appointment_fails_with_a_404_not_found_when_the_appointment_does_not_exist()
    {
        var client = _testFixture.Client;
        var id = Guid.NewGuid();
        var request = _fixture.Create<CancelAppointment>();
        var uri = $"{EndpointRoute}/{id}/cancel";

        var response = await client.PutAsJsonAsync(uri, request);

        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }
}