using System.Reflection;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using PetDoctor.Infrastructure;
using SqlStreamStore;

namespace PetDoctor.API.IntegrationTests.Setup
{
    public class TestStartup : Startup
    {
        public TestStartup(IConfiguration configuration) : base(configuration)
        {
        }

        protected override void ConfigureDatabaseServices(IServiceCollection services)
        {
            var cs = TestResources.Configuration.GetConnectionString("PetDoctorContext");

            services.AddDbContext<PetDoctorContext>(options =>
            {
                options.UseSqlServer(cs, sql =>
                {
                    sql.MigrationsAssembly(typeof(Startup).GetTypeInfo().Assembly.GetName().Name);
                });
            });

            services.AddSingleton(new MsSqlStreamStoreSettings(cs));
            services.AddSingleton<IStreamStore, MsSqlStreamStore>();
            services.AddSingleton<MsSqlStreamStore>(); // for migrations
        }
    }
}
