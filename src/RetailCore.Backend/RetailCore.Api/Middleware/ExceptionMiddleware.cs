using System.Net;

namespace RetailCore.Api.Middleware;

public class ExceptionMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<ExceptionMiddleware> _logger;

    public ExceptionMiddleware(RequestDelegate next, ILogger<ExceptionMiddleware> logger)
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
            await HandleExceptionAsync(context, ex);
        }
    }

    private Task HandleExceptionAsync(HttpContext context, Exception ex)
    {
        var statusCode = ex switch
        {
            KeyNotFoundException => HttpStatusCode.NotFound,
            InvalidOperationException => HttpStatusCode.BadRequest,
            _ => HttpStatusCode.InternalServerError
        };

        string message;
        if (statusCode == HttpStatusCode.InternalServerError)
        {
            message = "An unexpected error occurred.";
            _logger.LogError(ex, "Unhandled exception occurred");
        }
        else
        {
            message = ex.Message;
            _logger.LogWarning(ex.Message);
        }

        context.Response.ContentType = "application/json";
        context.Response.StatusCode = (int)statusCode;

        return context.Response.WriteAsJsonAsync(new
        {
            message,
            status = (int)statusCode
        });
    }
}
