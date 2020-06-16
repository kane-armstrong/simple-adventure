using FluentValidation.AspNetCore;
using MediatR;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using PetDoctor.Domain.Aggregates.Appointments;
using PetDoctor.Infrastructure;
using PetDoctor.Infrastructure.Repositories;
using SqlStreamStore;
using System;
using System.Reflection;

namespace PetDoctor.API
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        public void ConfigureServices(IServiceCollection services)
        {
            services.AddControllers()
                .AddNewtonsoftJson()
                .AddFluentValidation(options => options.RegisterValidatorsFromAssemblyContaining<Startup>())
                // Fixes 404s in TestServer
                .AddApplicationPart(typeof(Startup).Assembly);

            services.AddApiVersioning(options =>
            {
                options.ReportApiVersions = true;
                options.RegisterMiddleware = true;
            }).AddVersionedApiExplorer(options => { options.SubstituteApiVersionInUrl = true; });

            services.AddOpenApiDocument(document =>
            {
                document.DocumentName = "v1";
                document.ApiGroupNames = new[] { "1" };
                document.Title = "Pet Doctor API";
                document.Description = "This API enables managing veterinary appointments for your canine companions";
                document.Version = "v1";
                document.SerializerSettings = new JsonSerializerSettings { ContractResolver = new DefaultContractResolver() };
            });

            services.AddOpenApiDocument(document =>
            {
                document.DocumentName = "allVersions";
                document.Version = "allVersions";
                document.Title = "Pet Doctor API";
                document.Description = "This API enables managing veterinary appointments for your canine companions";
                document.SerializerSettings = new JsonSerializerSettings { ContractResolver = new DefaultContractResolver() };
            });

            services.AddHealthChecks();

            services.AddMediatR(Assembly.GetExecutingAssembly());

            services.AddTransient<IAppointmentRepository, AppointmentRepository>();

            ConfigureDatabaseServices(services);
        }

        public void ConfigureProductionServices(IServiceCollection services)
        {
            ConfigureServices(services);

            services.AddApplicationInsightsTelemetry(Configuration);

            var hostingEnvironment = Configuration.GetValue<string>("hostenv");
            if (!string.IsNullOrEmpty(hostingEnvironment) && hostingEnvironment.Equals("K8S", StringComparison.InvariantCultureIgnoreCase))
            {
                services.AddApplicationInsightsKubernetesEnricher();
            }
        }

        protected virtual void ConfigureDatabaseServices(IServiceCollection services)
        {
            var cs = Configuration.GetConnectionString("PetDoctorContext");

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

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            var pathBase = Configuration.GetValue<string>("PATH_BASE");
            if (!string.IsNullOrEmpty(pathBase))
                app.UsePathBase($"/{pathBase.TrimStart('/')}");

            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseRouting();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
                endpoints.MapHealthChecks("");
                endpoints.MapHealthChecks("live");
                endpoints.MapHealthChecks("ready");
            });

            app.UseOpenApi();
            app.UseSwaggerUi3();
        }
    }
}
