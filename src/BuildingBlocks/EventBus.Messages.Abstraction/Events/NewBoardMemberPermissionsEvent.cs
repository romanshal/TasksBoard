namespace EventBus.Messages.Abstraction.Events
{
    public class NewBoardMemberPermissionsEvent : BaseEvent
    {
        public required Guid BoardId { get; set; }
        public required string BoardName { get; set; }
        public required Guid SourceAccountId { get; set; }
        public required Guid AccountId { get; set; }
    }
}
