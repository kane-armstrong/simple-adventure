using Microsoft.AspNetCore.Authentication;
using System.Security.Claims;

namespace PetDoctor.API.IntegrationTests.Infrastructure.Auth.Authentication;

public class TestAuthenticationOptions : AuthenticationSchemeOptions
{
    public virtual ClaimsIdentity Identity { get; } = new ClaimsIdentity(new[]
    {
        new Claim(ClaimTypes.Name, Guid.NewGuid().ToString()),
    }, TestAuthenticationConstants.AuthenticationType);
}