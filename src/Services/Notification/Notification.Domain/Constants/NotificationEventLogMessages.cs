namespace Notification.Domain.Constants
{
    public static class NotificationEventLogMessages
    {
        public const string NoMemberIds = "No members ids for save in '{eventName}' event.";
        public const string Error = "Error when create new event with id '{messageId}'.";
        public const string Created = "Event with id '{messageId}' created.";
    }
}
