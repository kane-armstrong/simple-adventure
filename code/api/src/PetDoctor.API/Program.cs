using Microsoft.Azure.KeyVault;
using Microsoft.Azure.Services.AppAuthentication;
using Microsoft.Extensions.Configuration.AzureKeyVault;
using PetDoctor.API;
using Serilog;

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

    Log.Information("Starting host");
    CreateHostBuilder(args, configBuilder.Build()).Build().Run();
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
    .AddUserSecrets<Startup>(optional: true)
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
