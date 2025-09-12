using Common.Blocks.ValueObjects;

namespace Authentication.Domain.ValueObjects
{
    public sealed class ImageId : GuidValueObject<ImageId>
    {
        private ImageId(Guid value) : base(value) { }
    }
}
