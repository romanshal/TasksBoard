namespace EmailService.Core.Constants
{
    public enum OutboxStatuses
    {
        Pending,
        InProgress,
        Sent,
        Failed
    }
}
