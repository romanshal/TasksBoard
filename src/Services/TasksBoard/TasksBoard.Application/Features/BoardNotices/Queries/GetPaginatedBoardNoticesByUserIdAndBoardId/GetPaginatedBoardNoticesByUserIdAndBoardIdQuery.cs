﻿using TasksBoard.Application.DTOs;
using TasksBoard.Application.Models;

namespace TasksBoard.Application.Features.BoardNotices.Queries.GetPaginatedBoardNoticesByUserIdAndBoardId
{
    public record GetPaginatedBoardNoticesByUserIdAndBoardIdQuery : GetPaginatedListQuery<BoardNoticeDto>
    {
        public Guid UserId { get; set; }
        public Guid BoardId { get; set; }
    }
}
