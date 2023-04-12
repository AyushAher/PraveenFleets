using System.Net;
using System.Text.Json;
using Shared.Configuration;

namespace Api.Middlewares;

public class ErrorHandlerMiddleware
{
    private readonly ILogger _logger;
    private readonly RequestDelegate _next;

    public ErrorHandlerMiddleware(RequestDelegate next)
    {
        var loggerFactory = LoggerFactory
                    .Create(builder =>
                    {
                        builder.ClearProviders();
                        builder.AddConsole();
                    });
        _logger = loggerFactory.CreateLogger<ErrorHandlerMiddleware>();
        _next = next;
    }

    public async Task Invoke(HttpContext context)
    {
        try
        {
            await _next(context);
        }
        catch (Exception error)
        {
            var response = context.Response;
            response.ContentType = "application/json";
            var responseModel = await ApiResponse<object>.FatalAsync(error, _logger);

            switch (error)
            {
                case ApplicationException e:
                    // custom application error
                    response.StatusCode = (int)HttpStatusCode.BadRequest;
                    break;

                case KeyNotFoundException e:
                    // not found error
                    response.StatusCode = (int)HttpStatusCode.NotFound;
                    break;

                default:
                    // unhandled error
                    if (error.HResult == 401)
                        response.StatusCode = (int)HttpStatusCode.Unauthorized;
                    else
                        response.StatusCode = (int)HttpStatusCode.InternalServerError;
                    break;
            }
            var result = JsonSerializer.Serialize(responseModel);
            await response.WriteAsync(result);
        }
    }
}
