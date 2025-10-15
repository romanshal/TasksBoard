namespace EventBus.Messages.Abstraction.Events
{
    public abstract class BaseEvent
    {
        public Guid EventId { get; init; }
        public DateTimeOffset CreateDate { get; init; }
        public required IEnumerable<Guid> UsersInterested { get; set; }

        public BaseEvent()
        {
            EventId = Guid.NewGuid();
            CreateDate = DateTimeOffset.Now;
        }
    }
}
