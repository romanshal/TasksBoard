using Common.Blocks.ValueObjects;

namespace TasksBoard.Domain.ValueObjects
{
    public sealed class BoardImageId : GuidValueObject<BoardImageId>
    {
        private BoardImageId(Guid value) : base(value) { }
    }
}
