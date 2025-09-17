using AutoMapper;
using Common.Blocks.Extensions;
using Common.Blocks.Models;
using Common.Blocks.Models.DomainResults;
using Common.Blocks.ValueObjects;
using MediatR;
using Microsoft.Extensions.Logging;
using TasksBoard.Application.DTOs;
using TasksBoard.Application.Handlers;
using TasksBoard.Domain.Interfaces.UnitOfWorks;

namespace TasksBoard.Application.Features.BoardNotices.Queries.GetPaginatedBoardNoticesByUserId
{
    public class GetPaginatedBoardNoticesByUserIdQueryHandler(
        IUnitOfWork unitOfWork,
        ILogger<GetPaginatedBoardNoticesByUserIdQueryHandler> logger,
        IMapper mapper,
        IUserProfileHandler profileHandler) : IRequestHandler<GetPaginatedBoardNoticesByUserIdQuery, Result<PaginatedList<BoardNoticeDto>>>
    {
        private readonly IUnitOfWork _unitOfWork = unitOfWork;
        private readonly ILogger<GetPaginatedBoardNoticesByUserIdQueryHandler> _logger = logger;
        private readonly IMapper _mapper = mapper;
        private readonly IUserProfileHandler _profileHandler = profileHandler;

        public async Task<Result<PaginatedList<BoardNoticeDto>>> Handle(GetPaginatedBoardNoticesByUserIdQuery request, CancellationToken cancellationToken)
        {
            var accountId = AccountId.Of(request.UserId);

            var count = await _unitOfWork.GetBoardNoticeRepository().CountByUserIdAsync(accountId, cancellationToken);
            if (count == 0)
            {
                _logger.LogInformation("No board notices entities in database.");
                return Result.Success(PaginatedList<BoardNoticeDto>.Empty(request.PageIndex, request.PageSize));
            }

            var boardNotice = await _unitOfWork.GetBoardNoticeRepository().GetPaginatedByUserIdAsync(accountId, request.PageIndex, request.PageSize, cancellationToken);

            var boardNoticesDto = _mapper.Map<IEnumerable<BoardNoticeDto>>(boardNotice);

            await _profileHandler.Handle(
                boardNoticesDto,
                x => x.AuthorId,
                (x, username, _) => x.AuthorName = username,
                cancellationToken);

            return Result.Success(boardNoticesDto.ToPaginatedList(request.PageIndex, request.PageSize, count));
        }
    }
}
