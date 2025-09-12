using Common.Blocks.ValueObjects;

namespace Common.Outbox.ValueObjects
{
    public sealed class OutboxId : GuidValueObject<OutboxId>
    {
        private OutboxId(Guid value) : base(value) { }
    }
}
