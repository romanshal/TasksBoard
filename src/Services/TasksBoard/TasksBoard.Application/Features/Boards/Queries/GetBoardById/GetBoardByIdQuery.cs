using Common.Blocks.Models.DomainResults;
using MediatR;
using TasksBoard.Application.DTOs.Boards;

namespace TasksBoard.Application.Features.Boards.Queries.GetBoardById
{
    public record GetBoardByIdQuery : IRequest<Result<BoardFullDto>>
    {
        public required Guid Id { get; set; }
    }
}
