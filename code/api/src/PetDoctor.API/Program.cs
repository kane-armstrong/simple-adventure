using Microsoft.AspNetCore.Hosting;
using Microsoft.Azure.KeyVault;
using Microsoft.Azure.Services.AppAuthentication;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Configuration.AzureKeyVault;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using PetDoctor.API;
using PetDoctor.Infrastructure;
using Serilog;
using SqlStreamStore;
using System;
using System.IO;
using System.Threading.Tasks;

var currentEnvironment = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") ?? "Production";

try
{
    var configBuilder = CreateConfigurationBuilder();
    var loggerConfiguration = new LoggerConfiguration().ReadFrom.Configuration(configBuilder.Build());
    Log.Logger = loggerConfiguration.CreateLogger();

    Log.Logger.Information("Application starting");
    var isProd = currentEnvironment.ToLower() == "production";
    if (isProd)
    {
        Log.Logger.Information("Running in production; adding KeyVault as a configuration provider");
        AddKeyVaultConfigurationProvider(configBuilder);
    }

    var host = CreateHostBuilder(args, configBuilder.Build()).Build();

    Log.Information("Migrating databases");
    await MigrateDatabases(host);

    Log.Information("Starting host");
    host.Run();
    Log.Information("Stopped host");
}
catch (Exception e)
{
    Log.Error(e, "An error occurred while attempting to start the host: {message}", e.Message);
}
finally
{
    Log.CloseAndFlush();
}


IConfigurationBuilder CreateConfigurationBuilder() => new ConfigurationBuilder()
    .SetBasePath(Directory.GetCurrentDirectory())
    .AddJsonFile("appsettings.json", false, true)
    .AddJsonFile($"appsettings.{currentEnvironment}.json", true)
    .AddUserSecrets<Startup>()
    .AddEnvironmentVariables();

static IHostBuilder CreateHostBuilder(string[] args, IConfigurationRoot configuration) =>
    Host.CreateDefaultBuilder(args)
        .ConfigureWebHostDefaults(webBuilder =>
        {
            webBuilder.UseStartup<Startup>();
            webBuilder.UseSerilog();
            webBuilder.UseConfiguration(configuration);
        });

static void AddKeyVaultConfigurationProvider(IConfigurationBuilder builder)
{
    try
    {
        var cfg = builder.Build();
        var azureServiceTokenProvider = new AzureServiceTokenProvider();
        var keyVaultClient = new KeyVaultClient(new KeyVaultClient.AuthenticationCallback(azureServiceTokenProvider.KeyVaultTokenCallback));
        var kvUrl = cfg.GetValue<string>("keyvault:url");
        if (string.IsNullOrEmpty(kvUrl))
            throw new ArgumentException("A KeyVault URL is required (KEYVAULT__URL)");
        if (!Uri.IsWellFormedUriString(kvUrl, UriKind.Absolute))
            throw new ArgumentException($"Invalid KeyVault URI: {kvUrl} (must be a well formed URI string)");
        builder.AddAzureKeyVault(kvUrl, keyVaultClient, new DefaultKeyVaultSecretManager());
    }
    catch (Exception e)
    {
        Log.Logger.Error(e, "Failed to add KeyVault as a configuration provider.");
        throw;
    }
}

static async Task MigrateDatabases(IHost host)
{
    using var scope = host.Services.CreateScope();

    var streamStore = scope.ServiceProvider.GetRequiredService<MsSqlStreamStoreV3>();

    await streamStore.CreateSchemaIfNotExists();
}