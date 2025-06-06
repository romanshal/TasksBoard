namespace EventBus.Messages.Events
{
    public class NewBoardMemberEvent : BaseEvent
    {
        public required Guid BoardId { get; set; }
        public required string BoardName { get; set; }
        public required Guid AccountId { get; set; }
        public required string AccountName { get; set; }
        public required IList<Guid> BoardMembersIds { get; set; }
    }
}
