using Common.Blocks.ValueObjects;

namespace TasksBoard.Domain.ValueObjects
{
    public sealed class MemberPermissionId : GuidValueObject<MemberPermissionId>
    {
        private MemberPermissionId(Guid value) : base(value) { }
    }
}
