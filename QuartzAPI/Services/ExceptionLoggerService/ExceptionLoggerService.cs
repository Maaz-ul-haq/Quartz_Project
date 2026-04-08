using Serilog.Context;

namespace QuartzAPI.Services.ExceptionLoggerService
{
    public class ExceptionLoggerService(ILogger<ExceptionLoggerService> logger, IHttpContextAccessor httpContextAccessor) : IExceptionLoggerService
    {
        private readonly ILogger<ExceptionLoggerService> _logger = logger;
        private readonly IHttpContextAccessor _httpContextAccessor = httpContextAccessor;

        public void Log(Exception exception)
        {
            var userId = _httpContextAccessor.HttpContext?.User?.FindFirst("UserID")?.Value;
            var innerMessage = exception.InnerException?.Message;

            using (LogContext.PushProperty("UserID", (object?)userId ?? DBNull.Value))
            using (LogContext.PushProperty("InnerExceptionMessage", (object?)innerMessage ?? DBNull.Value))
            {
                _logger.LogError(exception.Message);
            }

            exception.Data["Logged"] = true;
        }
    }
}
