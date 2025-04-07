using LibraryManagement.Core.Models;
using System.Net;
using System.Text.Json;

namespace LibraryManagement.API.Middleware
{
    public class ExceptionMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<ExceptionMiddleware> _logger;
        private readonly IHostEnvironment _env;

        public ExceptionMiddleware(RequestDelegate next, ILogger<ExceptionMiddleware> logger, IHostEnvironment env)
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
            catch (ApiException ex)
            {
                _logger.LogError(ex, $"API Error: {ex.Message}");

                context.Response.ContentType = "application/json";
                context.Response.StatusCode = ex.StatusCode;

                var response = ApiResponse<object>.ErrorResponse(ex.Message);
                var json = JsonSerializer.Serialize(response);

                await context.Response.WriteAsync(json);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Unhandled Error: {ex.Message}");

                context.Response.ContentType = "application/json";
                context.Response.StatusCode = (int)HttpStatusCode.InternalServerError;

                var response = _env.IsDevelopment()
                    ? ApiResponse<object>.ErrorResponse($"Internal Server Error: {ex.Message}\n{ex.StackTrace}")
                    : ApiResponse<object>.ErrorResponse("Internal Server Error");

                var json = JsonSerializer.Serialize(response);

                await context.Response.WriteAsync(json);
            }
        }
    }
}