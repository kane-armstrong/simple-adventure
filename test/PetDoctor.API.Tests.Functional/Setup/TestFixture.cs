using Microsoft.Extensions.DependencyInjection;
using PetDoctor.Domain.Aggregates.Appointments;
using System;
using System.Net.Http;
using System.Threading.Tasks;

namespace PetDoctor.API.Tests.Functional.Setup
{
    public class TestFixture
    {
        private readonly TestApiFactory _webApplicationFactory;
        public HttpClient Client { get; }

        public TestFixture()
        {
            _webApplicationFactory = new TestApiFactory();
            Client = _webApplicationFactory.CreateClient();
        }

        public async Task<Appointment> FindAppointment(Guid id)
        {
            var appointments = _webApplicationFactory.Services.GetRequiredService<IAppointmentRepository>();
            return await appointments.Find(id);
        }
    }
}
