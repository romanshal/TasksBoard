using Microsoft.Extensions.Logging;

namespace EmailService.Core.Constants
{
    public static partial class RabbitMqLoggerMessages
    {
        [LoggerMessage(Level = LogLevel.Information, Message = "RabbitMQ listener started on queue {queue}.")]
        public static partial void StartLogging(ILogger logger, string queue);

        [LoggerMessage(Level = LogLevel.Debug, Message = "Flushed {count} messages to DB.")]
        public static partial void LogFlushSuccess(ILogger logger, int count);

        [LoggerMessage(Level = LogLevel.Warning, Message = "Received null or invalid payload, nack requeue=false.")]
        public static partial void LogNullValue(ILogger logger);

        [LoggerMessage(Level = LogLevel.Information, Message = "RabbitMQ listener stopped due to cancellation.")]
        public static partial void LogOperationCancelled(ILogger logger);

        [LoggerMessage(Level = LogLevel.Error, Message = "RabbitMQ listener failed.")]
        public static partial void LogError(ILogger logger, Exception exception);

        [LoggerMessage(Level = LogLevel.Error, Message = "Error processing rabbit message, nack/requeue.")]
        public static partial void LogProccessingError(ILogger logger, Exception exception);

        [LoggerMessage(Level = LogLevel.Error, Message = "Flushing retry attempt {attempt} after {delay}. Exception: {exception}")]
        public static partial void LogRetryAttempt(ILogger logger, Exception exception, int attempt, TimeSpan delay);

        [LoggerMessage(Level = LogLevel.Error, Message = "RabbitMQ listener error during batch flush.")]
        public static partial void LogBatchFlushError(ILogger logger, Exception exception);

        [LoggerMessage(Level = LogLevel.Information, Message = "Stopping RabbitMQ listener...")]
        public static partial void StopLogging(ILogger logger);
    }
}
