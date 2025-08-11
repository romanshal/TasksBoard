using Common.Blocks.Models.DomainResults;
using MediatR;
using Microsoft.Extensions.Logging;
using TasksBoard.Domain.Constants.Errors.DomainErrors;
using TasksBoard.Domain.Entities;
using TasksBoard.Domain.Interfaces.UnitOfWorks;

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
            return await _unitOfWork.TransactionAsync(async token =>
            {
                var board = await _unitOfWork.GetRepository<Board>().GetAsync(request.BoardId, token);
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

                var permissions = await _unitOfWork.GetRepository<Domain.Entities.BoardPermission>().GetAllAsync(token);
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

                var affectedRows = await _unitOfWork.SaveChangesAsync(token);
                if (affectedRows == 0 || member.Id == Guid.Empty)
                {
                    _logger.LogError("Can't add new board member.");
                    return Result.Failure<Guid>(BoardMemberErrors.CantCreate);

                    //throw new ArgumentException(nameof(member));
                }

                //TODO: add event message

                _logger.LogInformation("Board member with account id '{accountId}' added to board with id '{boardId}'.", member.AccountId, request.BoardId);

                return Result.Success(member.Id);
            }, cancellationToken);
        }
    }
}
