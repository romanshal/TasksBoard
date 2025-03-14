using Common.Blocks.Exceptions;
using MediatR;
using Microsoft.Extensions.Logging;
using TasksBoard.Domain.Entities;
using TasksBoard.Domain.Interfaces.UnitOfWorks;

namespace TasksBoard.Application.Features.ManageBoardMembers.Commands.AddBoardMember
{
    public class AddBoardMemberCommandHandler(
        IUnitOfWork unitOfWork,
        ILogger<AddBoardMemberCommandHandler> logger) : IRequestHandler<AddBoardMemberCommand, Guid>
    {
        private readonly IUnitOfWork _unitOfWork = unitOfWork;
        private readonly ILogger<AddBoardMemberCommandHandler> _logger = logger;

        public async Task<Guid> Handle(AddBoardMemberCommand request, CancellationToken cancellationToken)
        {
            var board = await _unitOfWork.GetRepository<Board>().GetAsync(request.BoardId, cancellationToken);
            if (board is null)
            {
                _logger.LogWarning($"Board with id '{request.BoardId}' not found.");
                throw new NotFoundException($"Board with id '{request.BoardId}' not found.");
            }

            var member = board.BoardMembers.FirstOrDefault(member => member.AccountId == request.UserId);
            if (member is not null)
            {
                _logger.LogInformation($"User with id '{request.UserId} is already exist in board '{request.BoardId}'.");
                throw new AlreadyExistException($"Member is already exist in board '{board.Name}'.");
            }

            member = new BoardMember
            {
                BoardId = request.BoardId,
                AccountId = request.UserId,
                BoardMemberPermissions = [.. request.Permissions.Select(permission => new BoardMemberPermission
                {
                    BoardPermissionId = permission
                })]
            };

            await _unitOfWork.GetBoardMemberRepository().Add(member, true, cancellationToken);

            if (member.Id == Guid.Empty)
            {
                _logger.LogError("Can't add new board member.");
                throw new ArgumentException(nameof(member));
            }

            _logger.LogInformation($"Board member with account id '{member.AccountId}' added to board with id '{request.BoardId}'.");

            return member.Id;
        }
    }
}
