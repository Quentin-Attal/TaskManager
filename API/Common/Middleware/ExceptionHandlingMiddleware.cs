using System.Text.Json;
using Microsoft.AspNetCore.Mvc;

namespace API.Common.Middleware;

public sealed class ExceptionHandlingMiddleware(
    ILogger<ExceptionHandlingMiddleware> logger,
    IHostEnvironment env) : IMiddleware
{
    private static readonly JsonSerializerOptions JsonOptions = new(JsonSerializerDefaults.Web);

    private readonly ILogger<ExceptionHandlingMiddleware> _logger = logger;
    private readonly IHostEnvironment _env = env;

    public async Task InvokeAsync(HttpContext context, RequestDelegate next)
    {
        try
        {
            await next(context);
        }
        catch (OperationCanceledException) when (context.RequestAborted.IsCancellationRequested)
        {
            context.Response.StatusCode = 499;
        }
        catch (Exception ex)
        {
            var traceId = context.TraceIdentifier;

            _logger.LogError(ex,
                "Unhandled exception. TraceId={TraceId} Path={Path} Method={Method}",
                traceId,
                context.Request.Path.Value,
                context.Request.Method);

            var (statusCode, title) = MapException(ex);

            var problem = new ProblemDetails
            {
                Status = statusCode,
                Title = title,
                Type = $"https://httpstatuses.com/{statusCode}",
                Instance = context.Request.Path
            };

            problem.Extensions["traceId"] = traceId;

            if (_env.IsDevelopment())
            {
                problem.Detail = ex.Message;
                problem.Extensions["exception"] = ex.GetType().FullName;
            }

            context.Response.Clear();
            context.Response.StatusCode = statusCode;
            context.Response.ContentType = "application/problem+json";

            await context.Response.WriteAsync(JsonSerializer.Serialize(problem, JsonOptions));
        }
    }

    private static (int StatusCode, string Title) MapException(Exception ex) =>
        ex switch
        {
            ArgumentException => (StatusCodes.Status400BadRequest, "Bad request"),
            InvalidOperationException => (StatusCodes.Status409Conflict, "Invalid operation"),

            UnauthorizedAccessException => (StatusCodes.Status401Unauthorized, "Unauthorized"),

            _ => (StatusCodes.Status500InternalServerError, "An unexpected error occurred")
        };
}