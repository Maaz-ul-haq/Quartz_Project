namespace QuartzAPI.Services.ExceptionLoggerService
{
    public interface IExceptionLoggerService
    {
        void Log(Exception exception);
    }
}
