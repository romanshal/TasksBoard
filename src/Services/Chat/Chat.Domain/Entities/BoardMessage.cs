using Common.Blocks.Entities;

namespace Chat.Domain.Entities
{
    public class BoardMessage : BaseEntity
    {
        public Guid BoardId { get; set; }
        public Guid MemberId { get; set; }
        public Guid AccountId { get; set; }
        public required string MemberNickname { get; set; }
        public required string Message { get; set; }

    }
}
