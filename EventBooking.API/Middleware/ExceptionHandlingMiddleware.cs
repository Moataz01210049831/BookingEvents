using EventBooking.Application.Common.Exceptions;
using System.Net;
using System.Text.Json;

namespace EventBooking.API.Middleware
{
    public class ExceptionHandlingMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<ExceptionHandlingMiddleware> _logger;

        public ExceptionHandlingMiddleware(RequestDelegate next, ILogger<ExceptionHandlingMiddleware> logger)
        {
            _next = next;
            _logger = logger;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            try
            {
                await _next(context);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "حصل خطأ غير متوقع");
                await HandleExceptionAsync(context, ex);
            }
        }

        private static Task HandleExceptionAsync(HttpContext context, Exception exception)
        {
            var (statusCode, message) = exception switch
            {
                NotFoundException => (HttpStatusCode.NotFound, exception.Message),
                ValidationException => (HttpStatusCode.BadRequest, exception.Message),
                ForbiddenException => (HttpStatusCode.Forbidden, exception.Message),
                UnauthorizedAccessException => (HttpStatusCode.Unauthorized, "غير مصرح لك بالوصول"),
                _ => (HttpStatusCode.InternalServerError, "حصل خطأ غير متوقع، حاول تاني لاحقًا")
            };

            var response = new ErrorResponse
            {
                StatusCode = (int)statusCode,
                Message = message
            };

            context.Response.ContentType = "application/json";
            context.Response.StatusCode = (int)statusCode;

            return context.Response.WriteAsync(JsonSerializer.Serialize(response));
        }
    }
}