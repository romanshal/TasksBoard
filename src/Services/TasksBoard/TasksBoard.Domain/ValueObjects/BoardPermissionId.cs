using Common.Blocks.ValueObjects;

namespace TasksBoard.Domain.ValueObjects
{
    public sealed class BoardPermissionId : GuidValueObject<BoardPermissionId>
    {
        private BoardPermissionId(Guid value) : base(value) { }
    }
}
