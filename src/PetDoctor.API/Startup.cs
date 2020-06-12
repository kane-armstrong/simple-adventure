using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

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
            services.AddControllers().AddNewtonsoftJson();

            services.AddApiVersioning(options =>
            {
                options.ReportApiVersions = true;
                options.RegisterMiddleware = true;
            }).AddVersionedApiExplorer(options => { options.SubstituteApiVersionInUrl = true; });

            services.AddOpenApiDocument(document =>
            {
                document.DocumentName = "v1";
                document.ApiGroupNames = new[] {"1"};
                document.Title = "Pet Doctor API";
                document.Description = "This API enables managing veterinary appointments for your canine companions";
                document.Version = "v1";
                document.SerializerSettings = new JsonSerializerSettings {ContractResolver = new DefaultContractResolver()};
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
        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseRouting();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
                endpoints.MapHealthChecks("");
            });

            app.UseOpenApi();
            app.UseSwaggerUi3();
        }
    }
}
