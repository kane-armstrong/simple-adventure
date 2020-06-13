﻿using AutoFixture;
using FluentAssertions;
using PetDoctor.API.Application.Commands;
using System;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;

namespace PetDoctor.API.Tests.Functional.Helpers
{
    public class AppointmentSeeder
    {
        public async Task<Guid> CreateAppointment(HttpClient client, CreateAppointment appointment)
        {
            const string route = "api/v1/appointments";
            var result = await client.PostAsJsonAsync(route, appointment);
            result.EnsureSuccessStatusCode();
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
