using Common.Blocks.ValueObjects;

namespace TasksBoard.Domain.ValueObjects
{
    public sealed class BoardId : GuidValueObject<BoardId>
    {
        private BoardId(Guid value) : base(value) { }

    }
}
