using Common.Blocks.ValueObjects;

namespace TasksBoard.Domain.ValueObjects
{
    public sealed class BoardTagId : GuidValueObject<BoardTagId>
    {
        private BoardTagId(Guid value) : base(value) { }
    }
}
