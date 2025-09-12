using Common.Blocks.ValueObjects;

namespace TasksBoard.Domain.ValueObjects
{
    public sealed class BoardAccessId : GuidValueObject<BoardAccessId>
    {
        private BoardAccessId(Guid value) : base(value) { }
    }
}
