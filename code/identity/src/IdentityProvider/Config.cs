using System.Collections.Generic;
using IdentityServer4.Models;

namespace IdentityProvider;

public static class Config
{
    public static IEnumerable<IdentityResource> IdentityResources =>
        new IdentityResource[]
        {
            new IdentityResources.OpenId()
        };

    public static IEnumerable<ApiResource> ApiResources =>
        new ApiResource[]
        {
            new ("api")
            {
                Scopes = new List<string> { "api" }
            }
        };

    public static IEnumerable<ApiScope> ApiScopes =>
        new ApiScope[]
        {
            new ("api")
        };

    public static IEnumerable<Client> Clients =>
        new[]
        {
            new Client
            {
                ClientId = "442D29D9-A314-4F50-BB5A-8A5E908DCB6B",
                ClientSecrets = new List<Secret>
                {
                    new("secret".Sha256())
                },
                AllowedScopes = new List<string>
                {
                    "api"
                },
                AllowedGrantTypes = new List<string>
                {
                    GrantType.ClientCredentials
                }
            }
        };
}