using System.ComponentModel.DataAnnotations;
using System.Security.Authentication;
using API.TransferModels;

namespace API.Middleware;

public class GlobalExeptionHandeler
{
    private readonly ILogger<GlobalExeptionHandeler> _logger;
    public readonly RequestDelegate _next;

    public GlobalExeptionHandeler(ILogger<GlobalExeptionHandeler> logger, RequestDelegate next)
    {
        _logger = logger;
        _next = next;
    }

    public async Task InvokeAsync(HttpContext http)
    {
        try
        {
            await _next.Invoke(http);
        }
        catch (Exception e)
        {
            await HandleExeptionAsync(http, e, _logger);
        }
    }

    private static Task HandleExeptionAsync(HttpContext http, Exception exception,
        ILogger<GlobalExeptionHandeler> logger)
    {
        http.Response.ContentType = "application/json";
        logger.LogError(exception, "{ExeptionMessage}", exception.Message);

        if (exception is ValidationException ||
            exception is ArgumentException ||
            exception is ArgumentNullException ||
            exception is ArgumentOutOfRangeException ||
            exception is InvalidCredentialException)
        {
            http.Response.StatusCode = StatusCodes.Status400BadRequest;
        }
        else if(exception is KeyNotFoundException)
        {
            http.Response.StatusCode = StatusCodes.Status404NotFound;
        }
        else if(exception is UnauthorizedAccessException)
        {
            http.Response.StatusCode = StatusCodes.Status403Forbidden;
        }
        else if (exception is NotSupportedException ||
                 exception is NotImplementedException)
        {
            http.Response.StatusCode = StatusCodes.Status501NotImplemented;
            return http.Response.WriteAsJsonAsync(new ResponseDto { MessageToClient = "unable to process request" });
        }

        return http.Response.WriteAsJsonAsync(new ResponseDto
        {
            MessageToClient = exception.Message
        });
    }
}