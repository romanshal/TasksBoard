using Common.Blocks.Entities;

namespace Authentication.Domain.Entities
{
    public class ApplicationUserImage : BaseEntity
    {
        public required Guid UserId { get; set; }
        public required string Extension { get; set; }
        public required byte[] Image { get; set; }

        public virtual ApplicationUser User { get; set; }
    }
}
