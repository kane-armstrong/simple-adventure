﻿using AutoFixture;
using FluentAssertions;
using PetDoctor.API.Application.Commands;
using PetDoctor.API.IntegrationTests.Helpers;
using PetDoctor.API.IntegrationTests.Setup;
using PetDoctor.Domain.Aggregates.Appointments;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using Xunit;

namespace PetDoctor.API.IntegrationTests.Controllers.AppointmentController.CancelAppointmentSpec
{
    [Collection(TestCollections.RealDatabaseTests)]
    public class A_successful_cancel_appointment_request
    {
        private const string EndpointRoute = "v1/appointments";

        private readonly TestFixture _testFixture;
        private readonly Fixture _fixture;

        public A_successful_cancel_appointment_request(TestFixture testFixture)
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
            var request = _fixture.Create<CancelAppointment>();
            var uri = $"{EndpointRoute}/{id}/cancel";

            var response = await client.PutAsJsonAsync(uri, request);
            await response.ThrowWithBodyIfUnsuccessfulStatusCode();

            response.StatusCode.Should().Be(HttpStatusCode.NoContent);
        }

        [Fact]
        [ResetDatabase]
        public async Task actually_cancels_the_appointment()
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
        public async Task captures_the_cancellation_reason()
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
    }
}
