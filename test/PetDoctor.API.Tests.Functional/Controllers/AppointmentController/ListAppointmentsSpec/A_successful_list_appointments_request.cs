using FluentAssertions;
using Microsoft.AspNetCore.Http;
using PetDoctor.API.Application.Models;
using PetDoctor.API.Tests.Functional.Helpers;
using PetDoctor.API.Tests.Functional.Setup;
using System.Threading.Tasks;
using Xunit;

namespace PetDoctor.API.Tests.Functional.Controllers.AppointmentController.ListAppointmentsSpec
{
    [Collection(TestCollections.RealDatabaseTests)]
    public class A_successful_list_appointments_request : IClassFixture<TestFixture>
    {
        private const string EndpointRoute = "api/v1/appointments";

        private readonly TestFixture _testFixture;

        public A_successful_list_appointments_request(TestFixture testFixture)
        {
            _testFixture = testFixture;
        }

        [Fact]
        [ResetDatabase]
        public async Task returns_200_ok()
        {
            var client = _testFixture.Client;
            var uri = $"{EndpointRoute}?index=1&size=5";

            var result = await client.GetAsync(uri);

            result.StatusCode.Should().Be(StatusCodes.Status200OK);
        }

        [Fact]
        [ResetDatabase]
        public async Task returns_an_empty_page_when_there_are_no_appointments()
        {
            var client = _testFixture.Client;
            var uri = $"{EndpointRoute}?index=1&size=5";

            var result = await client.GetAsync(uri);
            result.IsSuccessStatusCode.Should().BeTrue();

            var page = await result.GetPayload<Page<AppointmentView>>();
            page.Data.Should().BeEmpty();
            page.TotalCount.Should().Be(0);
            page.HasNextPage.Should().BeFalse();
        }

        [Fact]
        [ResetDatabase]
        public async Task returns_a_single_page_when_page_size_exceeds_appointment_count()
        {
            var client = _testFixture.Client;

            const int pageCount = 10;
            const int count = 5;

            var uri = $"{EndpointRoute}?index=1&size={pageCount}";

            var seeder = new AppointmentSeeder();
            for (var i = 0; i < count; i++)
            {
                await seeder.CreateAppointment(client);
            }

            var result = await client.GetAsync(uri);
            result.IsSuccessStatusCode.Should().BeTrue();

            var page = await result.GetPayload<Page<AppointmentView>>();
            page.Data.Should().NotBeEmpty();
            page.TotalCount.Should().Be(count);
            page.HasNextPage.Should().BeFalse();
        }
    }
}
