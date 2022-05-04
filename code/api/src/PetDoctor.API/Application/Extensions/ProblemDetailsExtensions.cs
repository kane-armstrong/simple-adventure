using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

namespace PetDoctor.API.Application.Extensions
{
    public static class ProblemDetailsExtensions
    {
        public static IActionResult CreateContentResponse(this ProblemDetails problem)
        {
            return new ContentResult
            {
                StatusCode = problem.Status,
                ContentType = "application/json+problem",
                Content = JsonConvert.SerializeObject(problem)
            };
        }
    }
}
