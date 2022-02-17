using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using PetDoctor.Migrations.WriteStore;
using Serilog;
using SqlStreamStore;

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

        var master = configuration.GetConnectionString("master");
        var db = configuration.GetConnectionString("WriteStore");
        var appDbName = configuration.GetValue<string>("appDbName");

        services.Configure<DatabaseConnectionOptions>(o =>
        {
            o.Master = master;
            o.WriteStore = db;
            o.ApplicationDatabaseName = appDbName;
        });

        services.AddSingleton(new MsSqlStreamStoreV3Settings(db));
        services.AddSingleton<IStreamStore, MsSqlStreamStoreV3>();
        services.AddSingleton<MsSqlStreamStoreV3>();

        services.AddLogging(options =>
        {
            options.AddSerilog(dispose: true);
        });

        services.AddHostedService<Migrator>();
    })
    .RunConsoleAsync();
