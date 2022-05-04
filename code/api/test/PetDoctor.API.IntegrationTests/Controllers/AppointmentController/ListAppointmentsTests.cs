using FluentAssertions;
using PetDoctor.API.Application.Models;
using PetDoctor.API.IntegrationTests.Helpers;
using PetDoctor.API.IntegrationTests.Setup;
using System.Net;
using Xunit;

namespace PetDoctor.API.IntegrationTests.Controllers.AppointmentController;

[Collection(TestCollections.RealDatabaseTests)]
public class ListAppointmentsTests : IClassFixture<TestFixture>
{
    private const string EndpointRoute = "v1/appointments";

    private readonly TestFixture _testFixture;

    public ListAppointmentsTests(TestFixture testFixture)
    {
        _testFixture = testFixture;
    }

    [Fact]
    [ResetDatabase]
    public async Task Successful_requests_return_200_ok()
    {
        var client = _testFixture.Client;
        var uri = $"{EndpointRoute}?index=1&size=5";

        var result = await client.GetAsync(uri);

        result.StatusCode.Should().Be(HttpStatusCode.OK);
    }

    [Fact]
    [ResetDatabase]
    public async Task An_empty_page_is_returned_when_there_are_no_appointments()
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
    public async Task A_single_page_is_returned_when_page_size_exceeds_appointment_count()
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
    public async Task Multiple_pages_are_returned_when_appointment_count_exceeds_page_size()
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
