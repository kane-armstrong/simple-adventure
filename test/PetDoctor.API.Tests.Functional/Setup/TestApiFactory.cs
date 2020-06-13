using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.PlatformAbstractions;
using System.IO;

namespace PetDoctor.API.Tests.Functional.Setup
{
    public sealed class TestApiFactory : WebApplicationFactory<Startup>
    {
        protected override IHostBuilder CreateHostBuilder()
        {
            return Host.CreateDefaultBuilder()
                .ConfigureWebHostDefaults(webBuilder =>
                {
                    webBuilder.UseStartup<TestStartup>();
                });
        }

        protected override void ConfigureWebHost(IWebHostBuilder builder)
        {
            var applicationPath = Path.GetFullPath(PlatformServices.Default.Application.ApplicationBasePath);
            builder.UseContentRoot(applicationPath).UseEnvironment("Development");
            base.ConfigureWebHost(builder);
        }
    }
}
