using AutoMapper;
using Common.Blocks.Models.DomainResults;
using MediatR;
using Microsoft.Extensions.Logging;
using TasksBoard.Application.DTOs;
using TasksBoard.Application.Handlers;
using TasksBoard.Domain.Constants.Errors.DomainErrors;
using TasksBoard.Domain.Interfaces.UnitOfWorks;
using TasksBoard.Domain.ValueObjects;

namespace TasksBoard.Application.Features.BoardMembers.Queries.GetBoardMembersByBoardId
{
    public class GetBoardMembersByBoardIdQueryHandler(
        IUnitOfWork unitOfWork,
        ILogger<GetBoardMembersByBoardIdQueryHandler> logger,
        IMapper mapper,
        IUserProfileHandler profileHandler) : IRequestHandler<GetBoardMembersByBoardIdQuery, Result<IEnumerable<BoardMemberDto>>>
    {
        private readonly IUnitOfWork _unitOfWork = unitOfWork;
        private readonly ILogger<GetBoardMembersByBoardIdQueryHandler> _logger = logger;
        private readonly IMapper _mapper = mapper;
        private readonly IUserProfileHandler _profileHandler = profileHandler;

        public async Task<Result<IEnumerable<BoardMemberDto>>> Handle(GetBoardMembersByBoardIdQuery request, CancellationToken cancellationToken)
        {
            var boardId = BoardId.Of(request.BoardId);

            var boardExist = await _unitOfWork.GetBoardRepository().ExistAsync(boardId, cancellationToken);
            if (!boardExist)
            {
                _logger.LogWarning("Board with id '{boardId}' not found.", request.BoardId);
                return Result.Failure<IEnumerable<BoardMemberDto>>(BoardErrors.NotFound);
            }

            var boardMembers = await _unitOfWork.GetBoardMemberRepository().GetByBoardIdAsync(boardId, cancellationToken);

            var boardMembersDto = _mapper.Map<IEnumerable<BoardMemberDto>>(boardMembers);

            await _profileHandler.Handle(
                boardMembersDto,
                x => x.AccountId,
                (x, username, _) => x.Nickname = username,
                cancellationToken);

            return Result.Success(boardMembersDto);
        }
    }
}
