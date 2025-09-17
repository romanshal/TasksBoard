using AutoMapper;
using Common.Blocks.Extensions;
using Common.Blocks.Models;
using Common.Blocks.Models.DomainResults;
using Common.Blocks.ValueObjects;
using MediatR;
using Microsoft.Extensions.Logging;
using TasksBoard.Application.DTOs;
using TasksBoard.Application.Handlers;
using TasksBoard.Domain.Constants.Errors.DomainErrors;
using TasksBoard.Domain.Interfaces.UnitOfWorks;
using TasksBoard.Domain.ValueObjects;

namespace TasksBoard.Application.Features.BoardNotices.Queries.GetPaginatedBoardNoticesByUserIdAndBoardId
{
    public class GetPaginatedBoardNoticesByUserIdAndBoardIdQueryHandler(
        IUnitOfWork unitOfWork,
        ILogger<GetPaginatedBoardNoticesByUserIdAndBoardIdQueryHandler> logger,
        IMapper mapper,
        IUserProfileHandler profileHandler) : IRequestHandler<GetPaginatedBoardNoticesByUserIdAndBoardIdQuery, Result<PaginatedList<BoardNoticeDto>>>
    {
        private readonly IUnitOfWork _unitOfWork = unitOfWork;
        private readonly ILogger<GetPaginatedBoardNoticesByUserIdAndBoardIdQueryHandler> _logger = logger;
        private readonly IMapper _mapper = mapper;
        private readonly IUserProfileHandler _profileHandler = profileHandler;

        public async Task<Result<PaginatedList<BoardNoticeDto>>> Handle(GetPaginatedBoardNoticesByUserIdAndBoardIdQuery request, CancellationToken cancellationToken)
        {
            var boardId = BoardId.Of(request.BoardId);
            var accountId = AccountId.Of(request.UserId);

            var boardExist = await _unitOfWork.GetBoardRepository().ExistAsync(boardId, cancellationToken);
            if (!boardExist)
            {
                _logger.LogWarning("Board with id '{boardId}' not found.", request.BoardId);
                return Result.Failure<PaginatedList<BoardNoticeDto>>(BoardErrors.NotFound);
            }

            var count = await _unitOfWork.GetBoardNoticeRepository().CountByBoardIdAndUserIdAsync(boardId, accountId, cancellationToken);
            if (count == 0)
            {
                _logger.LogInformation("No board notices entities in database.");
                return Result.Success(PaginatedList<BoardNoticeDto>.Empty(request.PageIndex, request.PageSize));
            }

            var boardNotices = await _unitOfWork.GetBoardNoticeRepository().GetPaginatedByUserIdAndBoardIdAsync(accountId, boardId, request.PageIndex, request.PageSize, cancellationToken);

            var boardNoticesDto = _mapper.Map<IEnumerable<BoardNoticeDto>>(boardNotices);

            await _profileHandler.Handle(
                boardNoticesDto,
                x => x.AuthorId,
                (x, username, _) => x.AuthorName = username,
                cancellationToken);

            return Result.Success(boardNoticesDto.ToPaginatedList(request.PageIndex, request.PageSize, count));
        }
    }
}
