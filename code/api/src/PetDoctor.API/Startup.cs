using FluentValidation.AspNetCore;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using PetDoctor.API.Application.Commands;
using PetDoctor.API.Application.DomainEventHandlers;
using PetDoctor.API.Application.Queries;
using PetDoctor.API.Options;
using PetDoctor.Domain.Aggregates.Appointments;
using PetDoctor.Domain.Aggregates.Appointments.Events;
using PetDoctor.Infrastructure;
using PetDoctor.Infrastructure.Cqrs;
using PetDoctor.Infrastructure.Repositories;
using SqlStreamStore;
using System;
using System.Reflection;

namespace PetDoctor.API;

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

        services.AddScoped<ListAppointmentsHandler>();
        services.AddScoped<GetAppointmentByIdHandler>();

        services.AddScoped<CancelAppointmentHandler>();
        services.AddScoped<CheckinToAppointmentHandler>();
        services.AddScoped<CompleteAppointmentHandler>();
        services.AddScoped<ConfirmAppointmentHandler>();
        services.AddScoped<CreateAppointmentHandler>();
        services.AddScoped<RejectAppointmentHandler>();
        services.AddScoped<RescheduleAppointmentHandler>();

        services.AddScoped<IEventHandler<AppointmentCanceled>, AppointmentCanceledHandler>();
        services.AddScoped<IEventHandler<AppointmentMembersCheckedIn>, AppointmentMembersCheckedInHandler>();
        services.AddScoped<IEventHandler<AppointmentCompleted>, AppointmentCompletedHandler>();
        services.AddScoped<IEventHandler<AppointmentConfirmed>, AppointmentConfirmedHandler>();
        services.AddScoped<IEventHandler<AppointmentCreated>, AppointmentCreatedHandler>();
        services.AddScoped<IEventHandler<AppointmentRejected>, AppointmentRejectedHandler>();
        services.AddScoped<IEventHandler<AppointmentRescheduled>, AppointmentRescheduledHandler>();

        services.AddSingleton<IEventDispatcher, EventDispatcher>();

        services.AddTransient<IAppointmentRepository, AppointmentRepository>();

        ConfigureDatabaseServices(services);

        ConfigureAuthentication(services);
    }

    protected virtual void ConfigureAuthentication(IServiceCollection services)
    {
        var authenticationOptions = Configuration.GetSection("Authentication").Get<AuthenticationOptions>();
        services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
            .AddJwtBearer(options =>
            {
                options.Authority = authenticationOptions.Authority;
                options.Audience = authenticationOptions.Audience;
                options.RequireHttpsMetadata = authenticationOptions.RequireHttps;
            });

        services.AddAuthorization(options =>
        {
            options.AddPolicy("api", policy =>
            {
                policy.RequireAuthenticatedUser();
                policy.RequireClaim("scope", "api");
            });
        });
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
        var readStore = Configuration.GetConnectionString("readstore");

        services.AddDbContext<PetDoctorContext>(options =>
        {
            options.UseSqlServer(readStore, sql =>
            {
                sql.MigrationsAssembly(typeof(Startup).GetTypeInfo().Assembly.GetName().Name);
            });
        });

        var writeStore = Configuration.GetConnectionString("writestore");

        services.AddSingleton(new MsSqlStreamStoreV3Settings(writeStore));
        services.AddSingleton<IStreamStore, MsSqlStreamStoreV3>();
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

        app.UseAuthentication();
        app.UseAuthorization();

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