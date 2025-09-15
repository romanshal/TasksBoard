using Common.Blocks.CQRS;
using Common.Blocks.Models.DomainResults;

namespace TasksBoard.Application.Features.ManageBoards.Commands.DeleteBoard
{
    public record DeleteBoardCommand : ICommand<Result>
    {
        public required Guid Id { get; set; }
        public required Guid AccountId { get; set; }
    }
}
