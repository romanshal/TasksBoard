using Common.Blocks.ValueObjects;

namespace TasksBoard.Domain.ValueObjects
{
    public sealed class BoardMemberId : GuidValueObject<BoardMemberId>
    {
        private BoardMemberId(Guid value) : base(value) { }
    }
}
