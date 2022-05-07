using AutoFixture;
using FluentAssertions;
using PetDoctor.API.Application.Commands;
using PetDoctor.API.Application.Models;
using PetDoctor.API.IntegrationTests.Helpers;
using PetDoctor.API.IntegrationTests.Setup;
using System.Net;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using Xunit;

namespace PetDoctor.API.IntegrationTests.Controllers.AppointmentController;

[Collection(TestCollections.RealDatabaseTests)]
public class GetAppointmentTests : IClassFixture<TestFixture>
{
    private const string EndpointRoute = "v1/appointments";

    private readonly TestFixture _testFixture;

    public GetAppointmentTests(TestFixture testFixture)
    {
        _testFixture = testFixture;
    }

    [Fact]
    [ResetDatabase]
    public async Task Successful_requests_return_200_ok()
    {
        var client = _testFixture.Client;

        var id = await AppointmentSeeder.CreateAppointment(client);

        var uri = $"{EndpointRoute}/{id}";

        using var result = await client.GetAsync(uri);

        result.StatusCode.Should().Be(HttpStatusCode.OK);
    }

    [Fact]
    [ResetDatabase]
    public async Task Response_includes_the_correct_pet_name()
    {
        var client = _testFixture.Client;
        var fixture = new Fixture();
        fixture.Customize(new CreateAppointmentCustomization());
        var appointment = fixture.Create<CreateAppointment>();
        var id = await AppointmentSeeder.CreateAppointment(client, appointment);
        var uri = $"{EndpointRoute}/{id}";

        using var result = await client.GetAsync(uri);

        var view = await result.GetPayload<AppointmentView>();
        view.Should().NotBeNull();

        view.Pet.Name.Should().Be(appointment.PetName);
    }

    [Fact]
    [ResetDatabase]
    public async Task Response_includes_the_correct_pet_date_of_birth()
    {
        var client = _testFixture.Client;
        var fixture = new Fixture();
        fixture.Customize(new CreateAppointmentCustomization());
        var appointment = fixture.Create<CreateAppointment>();
        var id = await AppointmentSeeder.CreateAppointment(client, appointment);
        var uri = $"{EndpointRoute}/{id}";

        using var result = await client.GetAsync(uri);

        var view = await result.GetPayload<AppointmentView>();
        view.Should().NotBeNull();

        view.Pet.DateOfBirth.Should().Be(appointment.PetDateOfBirth);
    }

    [Fact]
    [ResetDatabase]
    public async Task Response_includes_the_correct_pet_breed()
    {
        var client = _testFixture.Client;
        var fixture = new Fixture();
        fixture.Customize(new CreateAppointmentCustomization());
        var appointment = fixture.Create<CreateAppointment>();
        var id = await AppointmentSeeder.CreateAppointment(client, appointment);
        var uri = $"{EndpointRoute}/{id}";

        using var result = await client.GetAsync(uri);

        var view = await result.GetPayload<AppointmentView>();
        view.Should().NotBeNull();

        view.Pet.Breed.Should().Be(appointment.PetBreed);
    }

    [Fact]
    [ResetDatabase]
    public async Task Response_includes_the_correct_owner_first_name()
    {
        var client = _testFixture.Client;
        var fixture = new Fixture();
        fixture.Customize(new CreateAppointmentCustomization());
        var appointment = fixture.Create<CreateAppointment>();
        var id = await AppointmentSeeder.CreateAppointment(client, appointment);
        var uri = $"{EndpointRoute}/{id}";

        using var result = await client.GetAsync(uri);

        var view = await result.GetPayload<AppointmentView>();
        view.Should().NotBeNull();

        view.Owner.FirstName.Should().Be(appointment.OwnerFirstName);
    }

    [Fact]
    [ResetDatabase]
    public async Task Response_includes_the_correct_owner_last_name()
    {
        var client = _testFixture.Client;
        var fixture = new Fixture();
        fixture.Customize(new CreateAppointmentCustomization());
        var appointment = fixture.Create<CreateAppointment>();
        var id = await AppointmentSeeder.CreateAppointment(client, appointment);
        var uri = $"{EndpointRoute}/{id}";

        using var result = await client.GetAsync(uri);

        var view = await result.GetPayload<AppointmentView>();
        view.Should().NotBeNull();

        view.Owner.LastName.Should().Be(appointment.OwnerLastName);
    }

    [Fact]
    [ResetDatabase]
    public async Task Response_includes_the_correct_owner_phone()
    {
        var client = _testFixture.Client;
        var fixture = new Fixture();
        fixture.Customize(new CreateAppointmentCustomization());
        var appointment = fixture.Create<CreateAppointment>();
        var id = await AppointmentSeeder.CreateAppointment(client, appointment);
        var uri = $"{EndpointRoute}/{id}";

        using var result = await client.GetAsync(uri);

        var view = await result.GetPayload<AppointmentView>();
        view.Should().NotBeNull();

        view.Owner.Phone.Should().Be(appointment.OwnerPhone);
    }

    [Fact]
    [ResetDatabase]
    public async Task Response_includes_the_correct_owner_email()
    {
        var client = _testFixture.Client;
        var fixture = new Fixture();
        fixture.Customize(new CreateAppointmentCustomization());
        var appointment = fixture.Create<CreateAppointment>();
        var id = await AppointmentSeeder.CreateAppointment(client, appointment);
        var uri = $"{EndpointRoute}/{id}";

        using var result = await client.GetAsync(uri);

        var view = await result.GetPayload<AppointmentView>();
        view.Should().NotBeNull();

        view.Owner.Email.Should().Be(appointment.OwnerEmail);
    }

    [Fact]
    [ResetDatabase]
    public async Task Response_includes_the_correct_vet_id()
    {
        var client = _testFixture.Client;
        var fixture = new Fixture();
        fixture.Customize(new CreateAppointmentCustomization());
        var appointment = fixture.Create<CreateAppointment>();
        var id = await AppointmentSeeder.CreateAppointment(client, appointment);
        var uri = $"{EndpointRoute}/{id}";

        using var result = await client.GetAsync(uri);

        var view = await result.GetPayload<AppointmentView>();
        view.Should().NotBeNull();

        view.AttendingVeterinarianId.Should().Be(appointment.DesiredVeterinarianId);
    }

    [Fact]
    [ResetDatabase]
    public async Task Response_includes_the_correct_reason_for_visit()
    {
        var client = _testFixture.Client;
        var fixture = new Fixture();
        fixture.Customize(new CreateAppointmentCustomization());
        var appointment = fixture.Create<CreateAppointment>();
        var id = await AppointmentSeeder.CreateAppointment(client, appointment);
        var uri = $"{EndpointRoute}/{id}";

        using var result = await client.GetAsync(uri);

        var view = await result.GetPayload<AppointmentView>();
        view.Should().NotBeNull();

        view.ReasonForVisit.Should().Be(appointment.ReasonForVisit);
    }

    [Fact]
    [ResetDatabase]
    public async Task Response_includes_the_correct_desired_date()
    {
        var client = _testFixture.Client;
        var fixture = new Fixture();
        fixture.Customize(new CreateAppointmentCustomization());
        var appointment = fixture.Create<CreateAppointment>();
        var id = await AppointmentSeeder.CreateAppointment(client, appointment);
        var uri = $"{EndpointRoute}/{id}";

        using var result = await client.GetAsync(uri);

        var view = await result.GetPayload<AppointmentView>();
        view.Should().NotBeNull();

        view.ScheduledOn.Should().Be(appointment.DesiredDate);
    }

    [Fact]
    [ResetDatabase]
    public async Task A_404_not_found_response_is_returned_when_the_appointment_does_not_exist()
    {
        var client = _testFixture.Client;
        var uri = $"{EndpointRoute}/{Guid.NewGuid()}";

        using var result = await client.GetAsync(uri);

        result.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    [Fact]
    [ResetDatabase]
    public async Task The_correct_response_body_is_returned_when_the_appointment_does_not_exist()
    {
        var client = _testFixture.Client;
        var uri = $"{EndpointRoute}/{Guid.NewGuid()}";

        using var result = await client.GetAsync(uri);

        var body = await result.Content.ReadAsStringAsync();
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