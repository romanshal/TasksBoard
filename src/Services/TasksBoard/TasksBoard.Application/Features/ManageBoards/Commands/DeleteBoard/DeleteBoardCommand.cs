using Common.Blocks.Models.DomainResults;
using MediatR;

namespace TasksBoard.Application.Features.ManageBoards.Commands.DeleteBoard
{
    public record DeleteBoardCommand : IRequest<Result>
    {
        public required Guid Id { get; set; }
    }
}
