using BookNow.Application.Common.Exceptions;
using BookNow.Presentation.Models;
using System.Net;
using System.Text.Json;

namespace BookNow.Presentation.Middleware
{
    public class GlobalExceptionHandlingMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<GlobalExceptionHandlingMiddleware> _logger;
        private readonly IHostEnvironment _env;

        public GlobalExceptionHandlingMiddleware(RequestDelegate next, ILogger<GlobalExceptionHandlingMiddleware> logger, IHostEnvironment env)
        {
            _next = next;
            _logger = logger;
            _env = env;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            try
            {
                await _next(context);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An unhandled exception occurred");
                await HandleExceptionAsync(context, ex, _env.IsDevelopment());
            }
        }

        private static Task HandleExceptionAsync(HttpContext context, Exception exception, bool isDevelopment)
        {
            context.Response.ContentType = "application/json";
            
            var response = new ApiResponse(false, "An unexpected error occurred.");
            var statusCode = StatusCodes.Status500InternalServerError;

            switch (exception)
            {
                case UnauthorizedAccessException:
                    statusCode = StatusCodes.Status401Unauthorized;
                    response.Message = "You are not authorized to perform this operation.";
                    break;

                case ForbiddenAccessException:
                    statusCode = StatusCodes.Status403Forbidden;
                    response.Message = exception.Message;
                    break;

                case KeyNotFoundException:
                    statusCode = StatusCodes.Status404NotFound;
                    response.Message = "The requested resource was not found.";
                    break;

                case ArgumentException:
                case InvalidOperationException:
                    statusCode = StatusCodes.Status400BadRequest;
                    response.Message = exception.Message; // Safe since these are usually validation errors from our logic
                    break;
                
                case TaskCanceledException:
                    statusCode = StatusCodes.Status408RequestTimeout;
                    response.Message = "The request was timed out.";
                    break;

                default:
                    response.Message = "A server error occurred. Please contact support if the problem persists.";
                    break;
            }

            context.Response.StatusCode = statusCode;

            if (isDevelopment)
            {
                response.Errors.Add($"Exception: {exception.GetType().Name}");
                response.Errors.Add($"Message: {exception.Message}");
                response.Errors.Add($"StackTrace: {exception.StackTrace}");
            }
            else
            {
                // In production, we can provide a generic error reference for tracking
                response.Errors.Add($"TraceId: {context.TraceIdentifier}");
            }

            var options = new JsonSerializerOptions { PropertyNamingPolicy = JsonNamingPolicy.CamelCase };
            var jsonResponse = JsonSerializer.Serialize(response, options);
            
            return context.Response.WriteAsync(jsonResponse);
        }
    }
}
