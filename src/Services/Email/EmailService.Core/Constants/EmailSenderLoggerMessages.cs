using Microsoft.Extensions.Logging;

namespace EmailService.Core.Constants
{
    public static partial class EmailSenderLoggerMessages
    {
        [LoggerMessage(Level = LogLevel.Information, Message = "Email sender background service started.")]
        public static partial void StartLogging(ILogger logger);

        [LoggerMessage(Level = LogLevel.Information, Message = "Email sender background service stopped due to cancellation.")]
        public static partial void LogOperationCancelled(ILogger logger);

        [LoggerMessage(Level = LogLevel.Error, Message = "Email sender failed.")]
        public static partial void LogError(ILogger logger, Exception exception);

        [LoggerMessage(Level = LogLevel.Error, Message = "Failed to send email {EmailId}.")]
        public static partial void LogSendError(ILogger logger, Exception exception, string emailId);

        [LoggerMessage(Level = LogLevel.Debug, Message = "Email {EmailId} sent successfully.")]
        public static partial void LogSuccess(ILogger logger, string emailId);
    }
}
