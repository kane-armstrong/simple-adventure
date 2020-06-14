using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using PetDoctor.Infrastructure;
using Serilog;
using Serilog.Core;
using Serilog.Events;
using SqlStreamStore;
using System;
using System.IO;
using Microsoft.Azure.KeyVault;
using Microsoft.Azure.Services.AppAuthentication;
using Microsoft.Extensions.Configuration.AzureKeyVault;

namespace PetDoctor.API
{
    public class Program
    {
        public static void Main(string[] args)
        {
            try
            {
                Log.Logger = CreateLogger();

                var host = CreateHostBuilder(args).Build();

                Log.Information("Migrating database");
                MigrateDatabases(host);

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
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .ConfigureWebHostDefaults(webBuilder =>
                {
                    webBuilder.UseStartup<Startup>();
                    webBuilder.UseSerilog();
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

        private static readonly string CurrentEnvironment = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") ?? "Production";

        private static IConfiguration Configuration
        {
            get
            {
                var builder = new ConfigurationBuilder()
                    .SetBasePath(Directory.GetCurrentDirectory())
                    .AddJsonFile("appsettings.json", false, true)
                    .AddJsonFile($"appsettings.{CurrentEnvironment}.json", true)
                    .AddUserSecrets<Startup>()
                    .AddEnvironmentVariables();

                var isProd = CurrentEnvironment.ToLower() == "production";
                if (!isProd) 
                    return builder.Build();
                
                var cfg = builder.Build();
                var azureServiceTokenProvider = new AzureServiceTokenProvider();
                var keyVaultClient = new KeyVaultClient(new KeyVaultClient.AuthenticationCallback(azureServiceTokenProvider.KeyVaultTokenCallback));
                var kvUrl = cfg.GetValue<string>("kevault:url");
                if (string.IsNullOrEmpty(kvUrl))
                    throw new ArgumentException("A KeyVault URL is required (KEYVAULT__URL)");
                if (!Uri.IsWellFormedUriString(kvUrl, UriKind.Absolute))
                    throw new ArgumentException($"Invalid KeyVault URI: {kvUrl} (must be a well formed URI string)");
                builder.AddAzureKeyVault(kvUrl, keyVaultClient, new DefaultKeyVaultSecretManager());

                return builder.Build();
            }
        }

        private static Logger CreateLogger()
        {
            var loggerConfiguration = new LoggerConfiguration()
                .ReadFrom.Configuration(Configuration)
                .Enrich.FromLogContext()
                .MinimumLevel.Override("Microsoft", LogEventLevel.Warning)
                .MinimumLevel.Override("Microsoft.AspNetCore", LogEventLevel.Warning)
                .MinimumLevel.Override("System", LogEventLevel.Warning)
                .WriteTo.Console();

            return loggerConfiguration.CreateLogger();
        }
    }
}
