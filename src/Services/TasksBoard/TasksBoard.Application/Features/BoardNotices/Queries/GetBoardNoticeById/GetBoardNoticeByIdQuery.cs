using Common.Blocks.Models.DomainResults;
using Common.Cache.CQRS;
using TasksBoard.Application.DTOs;

namespace TasksBoard.Application.Features.BoardNotices.Queries.GetBoardNoticeById
{
    public record GetBoardNoticeByIdQuery : ICachebleQuery<BoardNoticeDto, Result<BoardNoticeDto>>
    {
        public required Guid Id { get; set; }
    }
}
