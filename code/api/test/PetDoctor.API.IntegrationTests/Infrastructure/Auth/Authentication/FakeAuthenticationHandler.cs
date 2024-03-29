﻿using Microsoft.AspNetCore.Authentication;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System.Security.Claims;
using System.Text.Encodings.Web;

namespace PetDoctor.API.IntegrationTests.Infrastructure.Auth.Authentication;

public class FakeAuthenticationHandler : AuthenticationHandler<FakeAuthenticationOptions>
{
    public FakeAuthenticationHandler(
        IOptionsMonitor<FakeAuthenticationOptions> options,
        ILoggerFactory logger,
        UrlEncoder encoder,
        ISystemClock clock
    ) : base(options, logger, encoder, clock)
    {
    }

    protected override Task<AuthenticateResult> HandleAuthenticateAsync()
    {
        var authenticationTicket = new AuthenticationTicket(
            new ClaimsPrincipal(Options.Identity),
            new AuthenticationProperties(),
            FakeAuthenticationConstants.Scheme);

        return Task.FromResult(AuthenticateResult.Success(authenticationTicket));
    }
}