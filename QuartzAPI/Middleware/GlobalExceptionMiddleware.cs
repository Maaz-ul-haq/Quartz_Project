using QuartzAPI.Services.ExceptionLoggerService;
using System.Net;

namespace QuartzAPI.Middleware
{
    public class GlobalExceptionMiddleware(RequestDelegate next)
    {
        private readonly RequestDelegate _next = next;

        public async Task InvokeAsync(HttpContext httpContext, IExceptionLoggerService exceptionLogger)
        {
            try
            {
                await _next(httpContext);
            }
            catch (Exception ex)
            {
                await HandleExceptionAsync(httpContext, ex, exceptionLogger);
            }
        }

        private static async Task HandleExceptionAsync(HttpContext context, Exception exception, IExceptionLoggerService exceptionLogger)
        {
            context.Response.ContentType = "application/json";

            string? responseMessage;

            switch (exception)
            {
                case UnauthorizedAccessException:
                    context.Response.StatusCode = (int)HttpStatusCode.Unauthorized;
                    responseMessage = "Sorry! You're not authorized";
                    break;
                default:
                    context.Response.StatusCode = (int)HttpStatusCode.InternalServerError;
                    responseMessage = "Internal Server Error";
                    break;
            }

            if (exception.Data["Logged"] is not true)
                exceptionLogger.Log(exception);

            await context.Response.WriteAsync(responseMessage);
        }
    }
}
