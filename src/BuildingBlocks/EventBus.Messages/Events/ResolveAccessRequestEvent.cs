namespace EventBus.Messages.Events
{
    public class ResolveAccessRequestEvent : BaseEvent
    {
        public required Guid BoardId { get; set; }
        public required string BoardName { get; set; }
        public required Guid AccountId { get; set; }
        public required Guid SourceAccountId { get; set; }
        public required bool Status { get; set; }
    }
}
