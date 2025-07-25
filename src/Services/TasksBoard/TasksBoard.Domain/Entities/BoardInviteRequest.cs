﻿using Common.Blocks.Entities;

namespace TasksBoard.Domain.Entities
{
    public class BoardInviteRequest : BaseEntity
    {
        public required Guid BoardId { get; set; }
        public required Guid FromAccountId { get; set; }
        public required string FromAccountName { get; set; }
        public required Guid ToAccountId { get; set; }
        public required string ToAccountName { get; set; }
        public required string ToAccountEmail { get; set; }
        public required int Status { get; set; }

        public virtual Board Board { get; set; }
    }
}
