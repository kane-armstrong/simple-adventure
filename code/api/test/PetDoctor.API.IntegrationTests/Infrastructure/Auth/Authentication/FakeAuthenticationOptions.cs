using Microsoft.AspNetCore.Authentication;
using System.Security.Claims;

namespace PetDoctor.API.IntegrationTests.Infrastructure.Auth.Authentication;

public class FakeAuthenticationOptions : AuthenticationSchemeOptions
{
    public virtual ClaimsIdentity Identity { get; } = new(new[]
    {
        new Claim(ClaimTypes.Name, Guid.NewGuid().ToString()),
    }, FakeAuthenticationConstants.AuthenticationType);
}