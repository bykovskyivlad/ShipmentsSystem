using System.Net;
using System.Text.Json;

namespace Shipments.Api.Middleware;

public class ExceptionHandlingMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<ExceptionHandlingMiddleware> _logger;

    public ExceptionHandlingMiddleware(RequestDelegate next, ILogger<ExceptionHandlingMiddleware> logger)
    {
        _next = next;
        _logger = logger;
    }

    public async Task Invoke(HttpContext context)
    {
        try
        {
            await _next(context);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Unhandled exception");

            var (statusCode, title) = ex switch
            {
                KeyNotFoundException => ((int)HttpStatusCode.NotFound, "Not Found"),
                InvalidOperationException => ((int)HttpStatusCode.BadRequest, "Bad Request"),
                UnauthorizedAccessException => ((int)HttpStatusCode.Forbidden, "Forbidden"),
                _ => ((int)HttpStatusCode.InternalServerError, "Internal Server Error")
            };

            context.Response.StatusCode = statusCode;
            context.Response.ContentType = "application/problem+json";

            var problem = new
            {
                type = "about:blank",
                title,
                status = statusCode,
                detail = ex.Message,
                traceId = context.TraceIdentifier
            };

            await context.Response.WriteAsync(JsonSerializer.Serialize(problem));
        }
    }
}