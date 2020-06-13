using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using PetDoctor.Infrastructure;
using SqlStreamStore;
using System;

namespace PetDoctor.API.Tests.Functional.Setup
{
    public class TestStartup : Startup
    {
        public TestStartup(IConfiguration configuration) : base(configuration)
        {
        }

        protected override void ConfigureDatabaseServices(IServiceCollection services)
        {
            services.AddEntityFrameworkInMemoryDatabase();
            services.AddDbContext<PetDoctorContext>(options => options.UseInMemoryDatabase($"petdoc-{Guid.NewGuid()}"));
            services.AddSingleton<IStreamStore, InMemoryStreamStore>();
        }
    }
}
