using MediatR;
using TasksBoard.Application.DTOs;

namespace TasksBoard.Application.Features.BoardNotices.Queries.GetBoardNoticeById
{
    public record GetBoardNoticeByIdQuery : IRequest<BoardNoticeDto>
    {
        public required Guid Id { get; set; }
    }
}
