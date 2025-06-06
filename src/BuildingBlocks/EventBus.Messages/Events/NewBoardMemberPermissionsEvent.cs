namespace EventBus.Messages.Events
{
    public class NewBoardMemberPermissionsEvent : BaseEvent
    {
        public required Guid BoardId { get; set; }
        public required string BoardName { get; set; }
        public required Guid SourceAccountId { get; set; }
        public required string SourceAccountName { get; set; }
        public required Guid AccountId { get; set; }
        public required string AccountName { get; set; }
        public required IList<Guid> BoardMembersIds { get; set; }
    }
}
