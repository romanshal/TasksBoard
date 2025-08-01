﻿using TasksBoard.Application.DTOs;
using TasksBoard.Application.Models;

namespace TasksBoard.Application.Features.Boards.Queries.GetPaginatedBoards
{
    public record GetPaginatedBoardsQuery : GetPaginatedListQuery<BoardForViewDto>
    {
    }
}
