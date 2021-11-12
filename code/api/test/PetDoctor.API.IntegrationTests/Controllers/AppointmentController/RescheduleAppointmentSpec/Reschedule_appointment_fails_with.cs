﻿using AutoFixture;
using FluentAssertions;
using PetDoctor.API.Application.Commands;
using PetDoctor.API.IntegrationTests.Helpers;
using PetDoctor.API.IntegrationTests.Setup;
using System;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using Xunit;

namespace PetDoctor.API.IntegrationTests.Controllers.AppointmentController.RescheduleAppointmentSpec
{
    [Collection(TestCollections.RealDatabaseTests)]
    public class Reschedule_appointment_fails_with
    {
        private const string EndpointRoute = "v1/appointments";

        private readonly TestFixture _testFixture;
        private readonly Fixture _fixture;

        public Reschedule_appointment_fails_with(TestFixture testFixture)
        {
            _testFixture = testFixture;
            _fixture = new Fixture();
            _fixture.Customize(new CreateAppointmentCustomization());
        }

        [Fact]
        [ResetDatabase]
        public async Task a_404_not_found_when_the_appointment_does_not_exist()
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
}
