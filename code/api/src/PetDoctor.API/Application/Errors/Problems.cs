using Microsoft.AspNetCore.Mvc;

namespace PetDoctor.API.Application.Errors
{
    public static class Problems
    {
        private const string DocsHost = "https://developer.mozilla.org/en-US/docs/Web/HTTP/Status";

        public static ProblemDetails NotFoundProblem(string instance) => new()
        {
            Detail = "The requested resource was not found",
            Type = $"{DocsHost}/{StatusCodes.Status404NotFound}",
            Status = StatusCodes.Status404NotFound,
            Title = "Not Found",
            Instance = instance
        };
    }
}
