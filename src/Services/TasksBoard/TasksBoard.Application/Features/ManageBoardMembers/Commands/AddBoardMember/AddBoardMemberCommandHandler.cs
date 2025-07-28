using Common.Blocks.Models.DomainResults;
using MediatR;
using Microsoft.Extensions.Logging;
using TasksBoard.Application.Interfaces.UnitOfWorks;
using TasksBoard.Domain.Constants.Errors.DomainErrors;
using TasksBoard.Domain.Entities;

namespace TasksBoard.Application.Features.ManageBoardMembers.Commands.AddBoardMember
{
    public class AddBoardMemberCommandHandler(
        IUnitOfWork unitOfWork,
        ILogger<AddBoardMemberCommandHandler> logger) : IRequestHandler<AddBoardMemberCommand, Result<Guid>>
    {
        private readonly IUnitOfWork _unitOfWork = unitOfWork;
        private readonly ILogger<AddBoardMemberCommandHandler> _logger = logger;

        public async Task<Result<Guid>> Handle(AddBoardMemberCommand request, CancellationToken cancellationToken)
        {
            var board = await _unitOfWork.GetRepository<Board>().GetAsync(request.BoardId, cancellationToken);
            if (board is null)
            {
                _logger.LogWarning("Board with id '{boardId}' not found.", request.BoardId);
                return Result.Failure<Guid>(BoardErrors.NotFound);

                //throw new NotFoundException($"Board with id '{request.BoardId}' not found.");
            }

            var member = board.BoardMembers.FirstOrDefault(member => member.AccountId == request.AccountId);
            if (member is not null)
            {
                _logger.LogInformation("User with id '{accountId} is already exist in board '{boardId}'.", request.AccountId, request.BoardId);
                return Result.Failure<Guid>(BoardMemberErrors.AlreadyExist(board.Name));

                //throw new AlreadyExistException($"Member is already exist in board '{board.Name}'.");
            }

            var permissions = await _unitOfWork.GetRepository<Domain.Entities.BoardPermission>().GetAllAsync(cancellationToken);
            if (!permissions.Any())
            {
                _logger.LogError("No permissions available.");
                return Result.Failure<Guid>(BoardPermissionErrors.NoPermissions);

                //throw new NotFoundException("No permissions available.");
            }

            var minLevelPermission = permissions.OrderBy(permission => permission.AccessLevel).FirstOrDefault()!;

            member = new BoardMember
            {
                BoardId = request.BoardId,
                AccountId = request.AccountId,
                Nickname = request.Nickname,
                BoardMemberPermissions =
                [
                    new BoardMemberPermission
                    {
                        BoardPermissionId = minLevelPermission.Id
                    }
                ]
            };

            _unitOfWork.GetBoardMemberRepository().Add(member);

            var affectedRows = await _unitOfWork.SaveChangesAsync(cancellationToken);
            if (affectedRows == 0 || member.Id == Guid.Empty)
            {
                _logger.LogError("Can't add new board member.");
                return Result.Failure<Guid>(BoardMemberErrors.CantCreate);

                //throw new ArgumentException(nameof(member));
            }

            _logger.LogInformation("Board member with account id '{accountId}' added to board with id '{boardId}'.", member.AccountId, request.BoardId);

            return Result.Success(member.Id);
        }
    }
}
