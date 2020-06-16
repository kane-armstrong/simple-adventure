using AutoFixture;
using FluentAssertions;
using Microsoft.AspNetCore.Http;
using PetDoctor.API.Application.Commands;
using PetDoctor.API.Tests.Functional.Helpers;
using PetDoctor.API.Tests.Functional.Setup;
using PetDoctor.Domain.Aggregates.Appointments;
using System.Net.Http;
using System.Threading.Tasks;
using Xunit;

namespace PetDoctor.API.Tests.Functional.Controllers.AppointmentController.Complete_appointment_spec
{
    [Collection(TestCollections.RealDatabaseTests)]
    public class A_successful_complete_appointment_request
    {
        private const string EndpointRoute = "v1/appointments";

        private readonly TestFixture _testFixture;
        private readonly Fixture _fixture;

        public A_successful_complete_appointment_request(TestFixture testFixture)
        {
            _testFixture = testFixture;
            _fixture = new Fixture();
            _fixture.Customize(new CreateAppointmentCustomization());
        }

        [Fact]
        [ResetDatabase]
        public async Task returns_204_no_content()
        {
            var client = _testFixture.Client;
            var seeder = new AppointmentSeeder();
            var id = await seeder.CreateAppointment(client);
            var request = _fixture.Create<CompleteAppointment>();
            var uri = $"{EndpointRoute}/{id}/complete";

            var response = await client.PutAsJsonAsync(uri, request);
            await response.ThrowWithBodyIfUnsuccessfulStatusCode();

            response.StatusCode.Should().Be(StatusCodes.Status204NoContent);
        }

        [Fact]
        [ResetDatabase]
        public async Task actually_completes_the_appointment()
        {
            var client = _testFixture.Client;
            var seeder = new AppointmentSeeder();
            var id = await seeder.CreateAppointment(client);
            var request = _fixture.Create<CompleteAppointment>();
            var uri = $"{EndpointRoute}/{id}/complete";

            var response = await client.PutAsJsonAsync(uri, request);
            await response.ThrowWithBodyIfUnsuccessfulStatusCode();

            var sut = await _testFixture.FindAppointment(id);
            sut.State.Should().Be(AppointmentState.Completed);
        }
    }
}
