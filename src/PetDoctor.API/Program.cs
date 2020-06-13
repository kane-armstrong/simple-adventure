using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using PetDoctor.Infrastructure;
using SqlStreamStore;

namespace PetDoctor.API
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var host = CreateHostBuilder(args).Build();

            MigrateDatabases(host);

            host.Run();
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .ConfigureWebHostDefaults(webBuilder =>
                {
                    webBuilder.UseStartup<Startup>();
                });

        private static void MigrateDatabases(IHost host)
        {
            using var scope = host.Services.CreateScope();
            var appDbContext = scope.ServiceProvider.GetRequiredService<PetDoctorContext>();
            // Ideally this would be done in a separate console app in prod (with version assertions here)
            appDbContext.Database.Migrate();

            var streamStore = scope.ServiceProvider.GetRequiredService<MsSqlStreamStore>();
            var schemaCheck = streamStore.CheckSchema().GetAwaiter().GetResult();
            if (!schemaCheck.IsMatch())
                streamStore.CreateSchema().GetAwaiter().GetResult();
        }
    }
}
