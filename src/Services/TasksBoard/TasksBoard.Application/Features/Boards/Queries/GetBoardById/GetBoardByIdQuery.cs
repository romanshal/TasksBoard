using Common.Blocks.Models.DomainResults;
using Common.Cache.CQRS;
using TasksBoard.Application.DTOs.Boards;

namespace TasksBoard.Application.Features.Boards.Queries.GetBoardById
{
    public record GetBoardByIdQuery : ICachebleQuery<BoardFullDto, Result<BoardFullDto>>
    {
        public required Guid Id { get; init; }
    }
}
