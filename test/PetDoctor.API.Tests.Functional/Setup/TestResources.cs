using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System.IO;

namespace PetDoctor.API.Tests.Functional.Setup
{
    public static class TestResources
    {

        private static IConfiguration _configuration;

        public static IConfiguration Configuration
        {
            get
            {
                if (_configuration == null)
                    BuildConfiguration();
                return _configuration;
            }
        }

        private static void BuildConfiguration()
        {
            _configuration = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json", false, true)
                .AddEnvironmentVariables()
                .Build();
        }

        private static IServiceScopeFactory _scopeFactory;

        public static IServiceScopeFactory ScopeFactory
        {
            get
            {
                if (_scopeFactory == null)
                {
                    var services = new ServiceCollection();
                    var startup = new Startup(Configuration);
                    startup.ConfigureServices(services);
                    var provider = services.BuildServiceProvider();
                    _scopeFactory = provider.GetService<IServiceScopeFactory>();
                }

                return _scopeFactory;
            }
        }
    }
}
