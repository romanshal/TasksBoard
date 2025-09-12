using Common.Blocks.ValueObjects;

namespace TasksBoard.Domain.ValueObjects
{
    public sealed class BoardInviteId : GuidValueObject<BoardInviteId>
    {
        private BoardInviteId(Guid value) : base(value) { }
    }
}
