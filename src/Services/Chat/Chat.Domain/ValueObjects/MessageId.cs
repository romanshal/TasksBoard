using Common.Blocks.ValueObjects;

namespace Chat.Domain.ValueObjects
{
    public sealed class MessageId : GuidValueObject<MessageId>
    {
        private MessageId(Guid value) : base(value) { }
    }
}
