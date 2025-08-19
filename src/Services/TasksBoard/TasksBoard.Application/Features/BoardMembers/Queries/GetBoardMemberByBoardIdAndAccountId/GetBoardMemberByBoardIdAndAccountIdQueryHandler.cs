using AutoMapper;
using Common.Blocks.Models.DomainResults;
using Common.gRPC.Interfaces.Services;
using MediatR;
using Microsoft.Extensions.Logging;
using TasksBoard.Application.DTOs;
using TasksBoard.Domain.Constants.Errors.DomainErrors;
using TasksBoard.Domain.Entities;
using TasksBoard.Domain.Interfaces.UnitOfWorks;

namespace TasksBoard.Application.Features.BoardMembers.Queries.GetBoardMemberByBoardIdAndAccountId
{
    public class GetBoardMemberByBoardIdAndAccountIdQueryHandler(
        IUnitOfWork unitOfWork,
        ILogger<GetBoardMemberByBoardIdAndAccountIdQueryHandler> logger,
        IMapper mapper,
        IUserProfileService profileService) : IRequestHandler<GetBoardMemberByBoardIdAndAccountIdQuery, Result<BoardMemberDto>>
    {
        private readonly IUnitOfWork _unitOfWork = unitOfWork;
        private readonly ILogger<GetBoardMemberByBoardIdAndAccountIdQueryHandler> _logger = logger;
        private readonly IMapper _mapper = mapper;
        private readonly IUserProfileService _profileService = profileService;

        public async Task<Result<BoardMemberDto>> Handle(GetBoardMemberByBoardIdAndAccountIdQuery request, CancellationToken cancellationToken)
        {
            var boardExist = await _unitOfWork.GetRepository<Board>().ExistAsync(request.BoardId, cancellationToken);
            if (!boardExist)
            {
                _logger.LogWarning("Board with id '{boardId}' was not found.", request.BoardId);
                return Result.Failure<BoardMemberDto>(BoardErrors.NotFound);

                //throw new NotFoundException($"Board with id '{request.BoardId}' not found.");
            }

            var boardMember = await _unitOfWork.GetBoardMemberRepository().GetByBoardIdAndAccountIdAsync(request.BoardId, request.AccountId, cancellationToken);
            if (boardMember is null)
            {
                _logger.LogWarning("Board member with account id '{accountId} in board with id '{boardId} was not found.", request.AccountId, request.BoardId);
                return Result.Failure<BoardMemberDto>(BoardMemberErrors.NotFound);

                //throw new NotFoundException($"Board member with account id '{request.AccountId} in board with id '{request.BoardId} not found.");
            }

            var boardMemberDto = _mapper.Map<BoardMemberDto>(boardMember);

            var userProfile = await _profileService.ResolveAsync(boardMemberDto.AccountId, cancellationToken);

            if (userProfile is not null)
            {
                boardMemberDto.Nickname = userProfile.Username;
            }

            return Result.Success(boardMemberDto);
        }
    }
}
