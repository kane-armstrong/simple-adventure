using AutoFixture;
using FluentAssertions;
using PetDoctor.API.Application.Commands;

namespace PetDoctor.API.IntegrationTests.Helpers;

public static class AppointmentSeeder
{
    public static async Task<Guid> CreateAppointment(HttpClient client, CreateAppointment appointment)
    {
        const string route = "v1/appointments";
        var result = await client.PostAsJsonAsync(route, appointment);
        await result.ThrowWithBodyIfUnsuccessfulStatusCode();
        var foundIdInLocationHeader = Guid.TryParse(result.Headers.Location?.AbsoluteUri.Split('/').Last(), out var id);
        foundIdInLocationHeader.Should().BeTrue();
        return id;
    }

    public static Task<Guid> CreateAppointment(HttpClient client)
    {
        var fixture = new Fixture();
        fixture.Customize(new CreateAppointmentCustomization());
        var appointment = fixture.Create<CreateAppointment>();
        return CreateAppointment(client, appointment);
    }
}