using App.Api.Models;
using App.Common.Exceptions;

namespace App.Api.Middleware
{
    public class ExceptionHandlingMiddleware(RequestDelegate next, ILogger<ExceptionHandlingMiddleware> logger)
    {
        private readonly RequestDelegate _next = next;
        private readonly ILogger<ExceptionHandlingMiddleware> _logger = logger;

        public async Task Invoke(HttpContext context)
        {
            try
            {
                await _next(context);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Unhandled exception occurred");
                context.Response.ContentType = "application/json";
                context.Response.StatusCode = ex switch
                {
                    RecordNotFoundException => StatusCodes.Status404NotFound,
                    ArgumentException => StatusCodes.Status400BadRequest,
                    _ => StatusCodes.Status500InternalServerError
                };
                var errorResponse = new ErrorResponse
                {
                    Error = ex.Message,
                    StatusCode = context.Response.StatusCode
                };
                await context.Response.WriteAsJsonAsync(errorResponse);
            }
        }
    }
}
