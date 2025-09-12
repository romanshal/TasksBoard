using AutoMapper;
using Common.Blocks.Models.DomainResults;
using MediatR;
using Microsoft.Extensions.Logging;
using TasksBoard.Application.DTOs;
using TasksBoard.Application.Handlers;
using TasksBoard.Domain.Constants.Errors.DomainErrors;
using TasksBoard.Domain.Interfaces.UnitOfWorks;
using TasksBoard.Domain.ValueObjects;

namespace TasksBoard.Application.Features.BoardMembers.Queries.GetBoardMemberByBoardIdAndAccountId
{
    public class GetBoardMemberByBoardIdAndAccountIdQueryHandler(
        IUnitOfWork unitOfWork,
        ILogger<GetBoardMemberByBoardIdAndAccountIdQueryHandler> logger,
        IMapper mapper,
        IUserProfileHandler profileHandler) : IRequestHandler<GetBoardMemberByBoardIdAndAccountIdQuery, Result<BoardMemberDto>>
    {
        private readonly IUnitOfWork _unitOfWork = unitOfWork;
        private readonly ILogger<GetBoardMemberByBoardIdAndAccountIdQueryHandler> _logger = logger;
        private readonly IMapper _mapper = mapper;
        private readonly IUserProfileHandler _profileHandler = profileHandler;

        public async Task<Result<BoardMemberDto>> Handle(GetBoardMemberByBoardIdAndAccountIdQuery request, CancellationToken cancellationToken)
        {
            var boardId = BoardId.Of(request.BoardId);

            var boardExist = await _unitOfWork.GetBoardRepository().ExistAsync(boardId, cancellationToken);
            if (!boardExist)
            {
                _logger.LogWarning("Board with id '{boardId}' was not found.", request.BoardId);
                return Result.Failure<BoardMemberDto>(BoardErrors.NotFound);
            }

            var boardMember = await _unitOfWork.GetBoardMemberRepository().GetByBoardIdAndAccountIdAsync(boardId, request.AccountId, cancellationToken);
            if (boardMember is null)
            {
                _logger.LogWarning("Board member with account id '{accountId} in board with id '{boardId} was not found.", request.AccountId, request.BoardId);
                return Result.Failure<BoardMemberDto>(BoardMemberErrors.NotFound);
            }

            var boardMemberDto = _mapper.Map<BoardMemberDto>(boardMember);

            await _profileHandler.Handle(
                boardMemberDto,
                x => x.AccountId,
                (x, username, _) => x.Nickname = username,
                cancellationToken);

            return Result.Success(boardMemberDto);
        }
    }
}
