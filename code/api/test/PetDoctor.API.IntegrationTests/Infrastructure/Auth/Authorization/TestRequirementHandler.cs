using Microsoft.AspNetCore.Authorization;

namespace PetDoctor.API.IntegrationTests.Infrastructure.Auth.Authorization;

public class TestRequirementHandler : AuthorizationHandler<TestRequirement>
{
    protected override Task HandleRequirementAsync(
        AuthorizationHandlerContext context,
        TestRequirement requirement)
    {
        context.Succeed(requirement);
        return Task.CompletedTask;
    }
}