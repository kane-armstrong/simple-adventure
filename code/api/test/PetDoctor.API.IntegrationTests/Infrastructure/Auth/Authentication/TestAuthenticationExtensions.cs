﻿using Microsoft.AspNetCore.Authentication;
using System;

namespace PetDoctor.API.IntegrationTests.Infrastructure.Auth.Authentication;

public static class TestAuthenticationExtensions
{
    public static AuthenticationBuilder AddTestAuthentication(
        this AuthenticationBuilder builder,
        Action<TestAuthenticationOptions> configureOptions = null)
    {
        return builder.AddScheme<TestAuthenticationOptions, TestAuthenticationHandler>(
            TestAuthenticationConstants.Scheme,
            TestAuthenticationConstants.DisplayName,
            configureOptions ?? (o => { })
        );
    }
}