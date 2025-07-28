using Common.Blocks.Models.DomainResults;
using MediatR;
using TasksBoard.Application.DTOs;

namespace TasksBoard.Application.Features.Boards.Queries.GetBoardById
{
    public record GetBoardByIdQuery : IRequest<Result<BoardDto>>
    {
        public required Guid Id { get; set; }
    }
}
