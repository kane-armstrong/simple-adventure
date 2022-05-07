using Microsoft.AspNetCore.Authentication;

namespace PetDoctor.API.IntegrationTests.Infrastructure.Auth.Authentication;

public static class FakeAuthenticationExtensions
{
    public static AuthenticationBuilder AddFakeAuthentication(
        this AuthenticationBuilder builder,
        Action<FakeAuthenticationOptions>? configureOptions = null)
    {
        return builder.AddScheme<FakeAuthenticationOptions, FakeAuthenticationHandler>(
            FakeAuthenticationConstants.Scheme,
            FakeAuthenticationConstants.DisplayName,
            configureOptions ?? (_ => { })
        );
    }
}