namespace EventBus.Messages.Abstraction.Events
{
    public class NewBoardAccessRequestEvent : BaseEvent
    {
        public required Guid BoardId { get; set; }
        public required string BoardName { get; set; }
        public required Guid AccountId { get; set; }
    }
}
