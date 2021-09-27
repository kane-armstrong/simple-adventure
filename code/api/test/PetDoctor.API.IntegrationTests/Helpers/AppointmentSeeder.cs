using System;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using AutoFixture;
using FluentAssertions;
using PetDoctor.API.Application.Commands;

namespace PetDoctor.API.IntegrationTests.Helpers
{
    public class AppointmentSeeder
    {
        public async Task<Guid> CreateAppointment(HttpClient client, CreateAppointment appointment)
        {
            const string route = "v1/appointments";
            var result = await client.PostAsJsonAsync(route, appointment);
            await result.ThrowWithBodyIfUnsuccessfulStatusCode();
            var foundIdInLocationHeader = Guid.TryParse(result.Headers.Location.AbsoluteUri.Split('/').Last(), out var id);
            foundIdInLocationHeader.Should().BeTrue();
            return id;
        }

        public Task<Guid> CreateAppointment(HttpClient client)
        {
            var fixture = new Fixture();
            fixture.Customize(new CreateAppointmentCustomization());
            var appointment = fixture.Create<CreateAppointment>();
            return CreateAppointment(client, appointment);
        }
    }
}
