using Microsoft.OpenApi.Any;
using Microsoft.OpenApi.Models;

namespace TicTacToe.Extensions
{
    public static class HealthEndpointExtensions
    {
        public static IEndpointConventionBuilder MapHealthEndpoint(this IEndpointRouteBuilder endpoints)
        {
            return endpoints
                .MapGet("/health", () => Results.Ok(new { status = "Healthy", utcNow = DateTime.UtcNow }))
                .WithName("Health")
                .WithTags("Health")
                .Produces(StatusCodes.Status200OK, typeof(object), "application/json")
                .WithOpenApi(operation =>
                {
                    operation.Summary = "Service health check";
                    operation.Description = "Returns the API health status and the current server UTC time.";

                    if (operation.Responses.TryGetValue("200", out var response))
                    {
                        response.Description = "Healthy status with current UTC timestamp.";
                        response.Content ??= new Dictionary<string, OpenApiMediaType>();
                        response.Content["application/json"] = new OpenApiMediaType
                        {
                            Schema = new OpenApiSchema
                            {
                                Type = "object",
                                Properties =
                                {
                                    ["status"] = new OpenApiSchema { Type = "string", Example = new OpenApiString("Healthy") },
                                    ["utcNow"] = new OpenApiSchema { Type = "string", Format = "date-time" }
                                }
                            }
                        };
                    }

                    return operation;
                });
        }
    }
}