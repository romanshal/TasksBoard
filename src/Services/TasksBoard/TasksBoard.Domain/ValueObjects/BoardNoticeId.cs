using Common.Blocks.ValueObjects;

namespace TasksBoard.Domain.ValueObjects
{
    public sealed class BoardNoticeId : GuidValueObject<BoardNoticeId>
    {
        private BoardNoticeId(Guid value) : base(value) { }
    }
}
