using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using System.Net;
using TicTacToe.Exceptions;

namespace TicTacToe.Middleware
{
    public sealed class ExceptionHandlingMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<ExceptionHandlingMiddleware> _logger;

        public ExceptionHandlingMiddleware(RequestDelegate next, ILogger<ExceptionHandlingMiddleware> logger)
        {
            _next = next;
            _logger = logger;
        }

        public async Task InvokeAsync(HttpContext httpContext)
        {
            try
            {
                await _next(httpContext);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An unhandled exception occurred while processing the request.");

                await HandleExceptionAsync(httpContext, ex);
            }
        }

        private static Task HandleExceptionAsync(HttpContext httpContext, Exception ex)
        {
            var (status, title) = ex switch
            {
                ArgumentException => ((int)HttpStatusCode.BadRequest, "Invalid request"),
                InvalidOperationException => ((int)HttpStatusCode.BadRequest, "Invalid operation"),
                UnauthorizedAccessException => ((int)HttpStatusCode.Unauthorized, "Unauthorized"),
                NotFoundException => ((int)HttpStatusCode.NotFound, "Not found"),
                _ => ((int)HttpStatusCode.InternalServerError, "An unexpected error occurred")
            };

            var problem = new ProblemDetails
            {
                Type = $"https://httpstatuses.io/{status}",
                Title = title,
                Status = status,
                Detail = ex.Message,
                Instance = httpContext.Request.Path
            };
            problem.Extensions["traceId"] = Activity.Current?.Id ?? httpContext.TraceIdentifier;

            httpContext.Response.ContentType = "application/problem+json";
            httpContext.Response.StatusCode = status;

            return httpContext.Response.WriteAsJsonAsync(problem);
        }
    }
}