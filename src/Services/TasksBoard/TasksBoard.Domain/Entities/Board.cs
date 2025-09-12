using Common.Blocks.Entities;
using TasksBoard.Domain.ValueObjects;

namespace TasksBoard.Domain.Entities
{
    public class Board : BaseEntity<BoardId>
    {
        public required Guid OwnerId { get; set; }
        public required string Name { get; set; }
        public string? Description { get; set; }
        public bool Public { get; set; }

        public virtual BoardImage? BoardImage { get; set; } = default!;
        public virtual ICollection<BoardMember> BoardMembers { get; set; } = default!;
        public virtual ICollection<BoardNotice> Notices { get; set; } = default!;
        public virtual ICollection<BoardTag> Tags { get; set; } = default!;
        public virtual ICollection<BoardAccessRequest> AccessRequests { get; set; } = default!;
        public virtual ICollection<BoardInviteRequest> InviteRequests { get; set; } = default!;
    }
}
