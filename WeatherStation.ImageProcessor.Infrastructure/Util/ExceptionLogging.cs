using Microsoft.Extensions.Logging;

namespace WeatherStation.ImageProcessor.Infrastructure.Util
{
    public static class ExceptionLogging
    {
        public static Task<T> ExecuteWithExceptionLoggingAsync<T>(
            this ILogger logger,
            Func<Task<T>> operation,
            string errorMessage,
            params object[] args)
        {
            return ExecuteWithExceptionLoggingAsync(
                operation,
                ex => logger.LogError(ex, errorMessage, args));
        }

        private static async Task<T> ExecuteWithExceptionLoggingAsync<T>(
            Func<Task<T>> operation,
            Action<Exception> logError)
        {
            try
            {
                return await operation();
            }
            catch (Exception ex)
            {
                logError(ex);
                throw;
            }
        }

        public static Task ExecuteWithExceptionLoggingAsync(
            this ILogger logger,
            Func<Task> operation,
            string errorMessage,
            params object[] args)
        {
            return ExecuteWithExceptionLoggingAsync(
                operation,
                ex => logger.LogError(ex, errorMessage, args));
        }

        private static async Task ExecuteWithExceptionLoggingAsync(
            Func<Task> operation,
            Action<Exception> logError)
        {
            try
            {
                await operation();
            }
            catch (Exception ex)
            {
                logError(ex);
                throw;
            }
        }
    }
}
