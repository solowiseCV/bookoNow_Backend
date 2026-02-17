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
            var response = new ApiResponse(false, "An error occurred");

            switch (exception)
            {
                case ArgumentNullException:
                    context.Response.StatusCode = StatusCodes.Status400BadRequest;
                    response.Message = "A required argument was null";
                    response.Errors.Add(exception.Message);
                    break;

                case ArgumentException:
                    context.Response.StatusCode = StatusCodes.Status400BadRequest;
                    response.Message = "Invalid argument provided";
                    response.Errors.Add(exception.Message);
                    break;

                case InvalidOperationException:
                    context.Response.StatusCode = StatusCodes.Status400BadRequest;
                    response.Message = "Invalid operation";
                    response.Errors.Add(exception.Message);
                    break;

                case UnauthorizedAccessException:
                    context.Response.StatusCode = StatusCodes.Status401Unauthorized;
                    response.Message = "Unauthorized access";
                    response.Errors.Add(exception.Message);
                    break;

                case KeyNotFoundException:
                    context.Response.StatusCode = StatusCodes.Status404NotFound;
                    response.Message = "Resource not found";
                    response.Errors.Add(exception.Message);
                    break;

                default:
                    context.Response.StatusCode = StatusCodes.Status500InternalServerError;
                    response.Message = "An internal server error occurred";
                    response.Errors.Add("Please try again later or contact support");
                    break;
            }

            if (isDevelopment)
            {
                response.Errors.Add($"Exception: {exception.Message}");
                response.Errors.Add($"StackTrace: {exception.StackTrace}");
            }

            var options = new JsonSerializerOptions { PropertyNamingPolicy = JsonNamingPolicy.CamelCase };
            var jsonResponse = JsonSerializer.Serialize(response, options);
            
            return context.Response.WriteAsync(jsonResponse);
        }
    }
}
