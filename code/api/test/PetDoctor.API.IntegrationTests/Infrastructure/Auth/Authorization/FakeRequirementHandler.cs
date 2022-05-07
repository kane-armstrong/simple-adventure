using Microsoft.AspNetCore.Authorization;

namespace PetDoctor.API.IntegrationTests.Infrastructure.Auth.Authorization;

public class FakeRequirementHandler : AuthorizationHandler<FakeRequirement>
{
    protected override Task HandleRequirementAsync(
        AuthorizationHandlerContext context,
        FakeRequirement requirement)
    {
        context.Succeed(requirement);
        return Task.CompletedTask;
    }
}