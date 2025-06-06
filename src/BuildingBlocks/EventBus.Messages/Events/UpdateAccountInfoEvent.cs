namespace EventBus.Messages.Events
{
    public class UpdateAccountInfoEvent : BaseEvent
    {
        public required Guid AccountId { get; set; }
        public required string AccountName { get; set; }
        public required string AccountEmail { get; set; }
        public string? AccountFirstname { get; set; }
        public string? AccountSurname { get; set; }
    }
}
