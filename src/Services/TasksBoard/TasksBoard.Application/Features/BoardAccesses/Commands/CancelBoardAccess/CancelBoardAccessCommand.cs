using Common.Blocks.Models.DomainResults;
using MediatR;

namespace TasksBoard.Application.Features.BoardAccesses.Commands.CancelBoardAccess
{
    public record CancelBoardAccessCommand : IRequest<Result<Guid>>
    {
        public required Guid RequestId { get; set; }
    }
}
