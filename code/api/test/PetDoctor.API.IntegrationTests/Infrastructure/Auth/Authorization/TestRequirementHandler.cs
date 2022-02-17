using Microsoft.AspNetCore.Authorization;
using System.Threading.Tasks;

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