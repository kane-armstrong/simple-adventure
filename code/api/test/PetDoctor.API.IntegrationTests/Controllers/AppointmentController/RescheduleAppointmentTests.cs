using AutoFixture;
using FluentAssertions;
using PetDoctor.API.Application.Commands;
using PetDoctor.API.IntegrationTests.Helpers;
using PetDoctor.API.IntegrationTests.Setup;
using PetDoctor.Domain.Aggregates.Appointments;
using System.Net;
using Xunit;

namespace PetDoctor.API.IntegrationTests.Controllers.AppointmentController;

[Collection(TestCollections.RealDatabaseTests)]
public class RescheduleAppointmentTests
{
    private const string EndpointRoute = "v1/appointments";

    private readonly TestFixture _testFixture;
    private readonly Fixture _fixture;

    public RescheduleAppointmentTests(TestFixture testFixture)
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
        var request = _fixture.Build<RescheduleAppointment>()
            .With(p => p.NewDate, () => DateTimeOffset.UtcNow.AddDays(3)).Create();
        var uri = $"{EndpointRoute}/{id}/reschedule";

        var response = await client.PutAsJsonAsync(uri, request);
        await response.ThrowWithBodyIfUnsuccessfulStatusCode();

        response.StatusCode.Should().Be(HttpStatusCode.NoContent);
    }

    [Fact]
    [ResetDatabase]
    public async Task Rescheduled_appointments_are_persisted_correctly()
    {
        var client = _testFixture.Client;
        var id = await AppointmentSeeder.CreateAppointment(client);
        var request = _fixture.Build<RescheduleAppointment>()
            .With(p => p.NewDate, () => DateTimeOffset.UtcNow.AddDays(3)).Create();
        var uri = $"{EndpointRoute}/{id}/reschedule";

        var response = await client.PutAsJsonAsync(uri, request);
        await response.ThrowWithBodyIfUnsuccessfulStatusCode();

        var sut = await _testFixture.FindAppointment(id);
        sut?.State.Should().Be(AppointmentState.Requested);
    }

    [Fact]
    [ResetDatabase]
    public async Task Rescheduling_an_appointment_captures_the_new_date_correctly()
    {
        var client = _testFixture.Client;
        var id = await AppointmentSeeder.CreateAppointment(client);
        var request = _fixture.Build<RescheduleAppointment>()
            .With(p => p.NewDate, () => DateTimeOffset.UtcNow.AddDays(3)).Create();
        var uri = $"{EndpointRoute}/{id}/reschedule";

        var response = await client.PutAsJsonAsync(uri, request);
        await response.ThrowWithBodyIfUnsuccessfulStatusCode();

        var sut = await _testFixture.FindAppointment(id);
        sut?.ScheduledOn.Should().Be(request.NewDate);
    }

    [Fact]
    [ResetDatabase]
    public async Task Rescheduling_an_appointment_fails_with_a_404_not_found_when_the_appointment_does_not_exist()
    {
        var client = _testFixture.Client;
        var id = Guid.NewGuid();
        var request = _fixture.Build<RescheduleAppointment>()
            .With(p => p.NewDate, () => DateTimeOffset.UtcNow.AddDays(3)).Create();
        var uri = $"{EndpointRoute}/{id}/reschedule";

        var response = await client.PutAsJsonAsync(uri, request);

        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }
}