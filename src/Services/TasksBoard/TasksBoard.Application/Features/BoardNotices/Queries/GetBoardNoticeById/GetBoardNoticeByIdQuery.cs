using Common.Blocks.Models.DomainResults;
using MediatR;
using TasksBoard.Application.DTOs;

namespace TasksBoard.Application.Features.BoardNotices.Queries.GetBoardNoticeById
{
    public record GetBoardNoticeByIdQuery : IRequest<Result<BoardNoticeDto>>
    {
        public required Guid Id { get; set; }
    }
}
