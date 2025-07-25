﻿using MediatR;
using TasksBoard.Application.DTOs;

namespace TasksBoard.Application.Features.BoardAccesses.Queries.GetBoardAccessRequestByAccountId
{
    public record GetBoardAccessRequestByAccountIdQuery : IRequest<IEnumerable<BoardAccessRequestDto>>
    {
        public required Guid AccountId { get; set; }
    }
}
