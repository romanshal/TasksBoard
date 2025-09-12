using Common.Blocks.ValueObjects;

namespace Notification.Domain.ValueObjects
{
    public sealed class ApplicationEventId : GuidValueObject<ApplicationEventId>
    {
        private ApplicationEventId(Guid value) : base(value) { }
    }
}
