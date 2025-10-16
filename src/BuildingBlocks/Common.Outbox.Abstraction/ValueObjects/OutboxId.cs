using Common.Blocks.ValueObjects;

namespace Common.Outbox.Abstraction.ValueObjects
{
    public sealed class OutboxId : GuidValueObject<OutboxId>
    {
        private OutboxId(Guid value) : base(value) { }
    }
}
