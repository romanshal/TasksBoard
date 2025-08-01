﻿using Common.Blocks.Models.DomainResults;
using MediatR;

namespace TasksBoard.Application.Features.ManageBoardNotices.Commands.UpdateBoardNoticeStatus
{
    public record UpdateBoardNoticeStatusCommand : IRequest<Result<Guid>>
    {
        public required Guid BoardId { get; set; }
        public required Guid AccountId { get; set; }
        public required string AccountName { get; set; }
        public required Guid NoticeId { get; set; }
        public bool Complete { get; set; }
    }
}
