using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using PetDoctor.Infrastructure;
using PetDoctor.Migrations.ReadStore;
using Serilog;
using System.Reflection;

await new HostBuilder()
    .ConfigureAppConfiguration((_, builder) =>
    {
        builder
            .SetBasePath(Directory.GetCurrentDirectory())
            .AddJsonFile("appsettings.json", false, true)
            .AddUserSecrets<Program>(true)
            .AddEnvironmentVariables();
    })
    .ConfigureServices((context, services) =>
    {
        var configuration = context.Configuration;

        Log.Logger = new LoggerConfiguration()
            .ReadFrom.Configuration(configuration)
            .Enrich.FromLogContext()
            .CreateLogger();

        var readStore = configuration.GetConnectionString("Database");

        services.AddDbContext<PetDoctorContext>(options =>
        {
            options.UseSqlServer(readStore, sql =>
            {
                sql.MigrationsAssembly(typeof(Program).GetTypeInfo().Assembly.GetName().Name);
            });
        });

        services.AddLogging(options =>
        {
            options.AddSerilog(dispose: true);
        });

        services.AddHostedService<Migrator>();
    })
    .RunConsoleAsync();
