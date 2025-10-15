namespace EventBus.Messages.Abstraction.Events
{
    public class RemoveBoardMemberEvent : BaseEvent
    {
        public required Guid BoardId { get; set; }
        public required string BoardName { get; set; }
        public required Guid RemovedAccountId { get; set; }
        public required Guid RemoveByAccountId { get; set; }
    }
}
