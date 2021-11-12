using FluentAssertions;
using PetDoctor.API.Application.Models;
using PetDoctor.API.IntegrationTests.Helpers;
using PetDoctor.API.IntegrationTests.Setup;
using System.Net;
using System.Threading.Tasks;
using Xunit;

namespace PetDoctor.API.IntegrationTests.Controllers.AppointmentController.ListAppointmentsSpec
{
    [Collection(TestCollections.RealDatabaseTests)]
    public class A_successful_list_appointments_request : IClassFixture<TestFixture>
    {
        private const string EndpointRoute = "v1/appointments";

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

            result.StatusCode.Should().Be(HttpStatusCode.OK);
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
            page.Data.Count.Should().Be(count);
            page.TotalCount.Should().Be(count);
            page.HasNextPage.Should().BeFalse();
        }

        [Fact]
        [ResetDatabase]
        public async Task returns_a_multiple_pages_when_appointment_count_exceeds_page_size()
        {
            var client = _testFixture.Client;

            const int pageCount = 5;
            const int count = 10;

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
            page.Data.Count.Should().Be(pageCount);
            page.TotalCount.Should().Be(count);
            page.HasNextPage.Should().BeTrue();
        }
    }
}
