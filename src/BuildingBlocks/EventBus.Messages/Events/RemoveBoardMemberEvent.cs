namespace EventBus.Messages.Events
{
    public class RemoveBoardMemberEvent : BaseEvent
    {
        public required Guid BoardId { get; set; }
        public required string BoardName { get; set; }
        public required Guid RemovedAccountId{ get; set; }
        public required string RemovedAccountName { get; set; }
        public required Guid RemoveByAccountId { get; set; }
        public required string RemoveByAccountName { get; set; }
        public required IList<Guid> BoardMembersIds{ get; set; }
    }
}
