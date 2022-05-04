using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace PetDoctor.API.IntegrationTests.Setup;

public static class TestResources
{
    private static IConfiguration? _configuration;

    public static IConfiguration Configuration
    {
        get { return _configuration ??= BuildConfiguration(); }
    }

    private static IConfiguration BuildConfiguration()
    {
        return new ConfigurationBuilder()
            .SetBasePath(Directory.GetCurrentDirectory())
            .AddJsonFile("appsettings.json", false, true)
            .AddEnvironmentVariables()
            .Build();
    }

    private static IServiceScopeFactory? _scopeFactory;

    public static IServiceScopeFactory ScopeFactory
    {
        get
        {
            if (_scopeFactory != null)
            {
                return _scopeFactory;
            }

            var services = new ServiceCollection();
            var startup = new TestStartup(Configuration);
            startup.ConfigureServices(services);
            var provider = services.BuildServiceProvider();
            _scopeFactory = provider.GetRequiredService<IServiceScopeFactory>();

            return _scopeFactory;
        }
    }
}