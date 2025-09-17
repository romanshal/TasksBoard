using Common.Blocks.Entities;
using Common.Blocks.ValueObjects;
using TasksBoard.Domain.ValueObjects;

namespace TasksBoard.Domain.Entities
{
    public class Board : BaseEntity<BoardId>
    {
        public required AccountId OwnerId { get; set; }
        public required string Name { get; set; }
        public string? Description { get; set; }
        public bool Public { get; set; }

        public virtual BoardImage? BoardImage { get; set; } = default!;
        public virtual ICollection<BoardMember> BoardMembers { get; set; } = default!;
        public virtual ICollection<BoardNotice> BoardNotices { get; set; } = default!;
        public virtual ICollection<BoardTag> BoardTags { get; set; } = default!;
        public virtual ICollection<BoardAccessRequest> BoardAccessRequests { get; set; } = default!;
        public virtual ICollection<BoardInviteRequest> BoardInviteRequests { get; set; } = default!;
    }
}
