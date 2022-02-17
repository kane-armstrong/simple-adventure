using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.DependencyInjection;
using PetDoctor.API.IntegrationTests.Infrastructure.Auth.Authentication;
using PetDoctor.API.IntegrationTests.Infrastructure.Auth.Authorization;

namespace PetDoctor.API.IntegrationTests.Infrastructure.Auth;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddFakeAuthorization(this IServiceCollection services, params string[] policies)
    {
        services.AddAuthorization(options =>
        {
            foreach (var policy in policies)
            {
                options.AddPolicy(policy, p => p.Requirements.Add(new TestRequirement()));
            }
        });
        services.AddSingleton<IAuthorizationHandler, TestRequirementHandler>();
        return services;
    }

    public static IServiceCollection AddFakeAuthentication(this IServiceCollection services)
    {
        services.AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = TestAuthenticationConstants.Scheme;
                options.DefaultChallengeScheme = TestAuthenticationConstants.Scheme;
            })
            .AddTestAuthentication();
        return services;
    }
}