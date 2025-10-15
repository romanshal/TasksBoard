namespace EventBus.Messages.Abstraction.Events
{
    public class UpdateNoticeStatusEvent : BaseEvent
    {
        public required Guid BoardId { get; set; }
        public required string BoardName { get; set; }
        public required Guid NoticeId { get; set; }
        public required Guid AccountId { get; set; }
        public bool Completed { get; set; }
    }
}
