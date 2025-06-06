namespace EventBus.Messages.Events
{
    public abstract class BaseEvent
    {
        public Guid RequestId { get; private set; }

        public DateTime CreationDate { get; private set; }

        public BaseEvent()
        {
            RequestId = Guid.NewGuid();
            CreationDate = DateTime.UtcNow;
        }

        public BaseEvent(Guid id, DateTime createDate)
        {
            RequestId = id;
            CreationDate = createDate;
        }
    }
}
