using AutoMapper;
using Common.Blocks.Extensions;
using Common.Blocks.Models;
using Common.Blocks.Models.DomainResults;
using Common.gRPC.Interfaces.Services;
using MediatR;
using Microsoft.Extensions.Logging;
using TasksBoard.Application.DTOs;
using TasksBoard.Domain.Constants.Errors.DomainErrors;
using TasksBoard.Domain.Entities;
using TasksBoard.Domain.Interfaces.UnitOfWorks;

namespace TasksBoard.Application.Features.BoardNotices.Queries.GetPaginatedBoardNoticesByUserIdAndBoardId
{
    public class GetPaginatedBoardNoticesByUserIdAndBoardIdQueryHandler(
        IUnitOfWork unitOfWork,
        ILogger<GetPaginatedBoardNoticesByUserIdAndBoardIdQueryHandler> logger,
        IMapper mapper,
        IUserProfileService profileService) : IRequestHandler<GetPaginatedBoardNoticesByUserIdAndBoardIdQuery, Result<PaginatedList<BoardNoticeDto>>>
    {
        private readonly IUnitOfWork _unitOfWork = unitOfWork;
        private readonly ILogger<GetPaginatedBoardNoticesByUserIdAndBoardIdQueryHandler> _logger = logger;
        private readonly IMapper _mapper = mapper;
        private readonly IUserProfileService _profileService = profileService;

        public async Task<Result<PaginatedList<BoardNoticeDto>>> Handle(GetPaginatedBoardNoticesByUserIdAndBoardIdQuery request, CancellationToken cancellationToken)
        {
            var boardExist = await _unitOfWork.GetRepository<Board>().ExistAsync(request.BoardId, cancellationToken);
            if (!boardExist)
            {
                _logger.LogWarning("Board with id '{boardId}' not found.", request.BoardId);
                return Result.Failure<PaginatedList<BoardNoticeDto>>(BoardErrors.NotFound);

                //throw new NotFoundException($"Board with id '{request.BoardId}' not found.");
            }

            var count = await _unitOfWork.GetBoardNoticeRepository().CountByBoardIdAndUserIdAsync(request.BoardId, request.UserId, cancellationToken);
            if (count == 0)
            {
                _logger.LogInformation("No board notices entities in database.");
                return Result.Success(new PaginatedList<BoardNoticeDto>([], request.PageIndex, request.PageSize));
            }

            var boardNotices = await _unitOfWork.GetBoardNoticeRepository().GetPaginatedByUserIdAndBoardIdAsync(request.UserId, request.BoardId, request.PageIndex, request.PageSize, cancellationToken);

            var boardNoticesDto = _mapper.Map<IEnumerable<BoardNoticeDto>>(boardNotices);

            var userIds = boardNoticesDto
                .SelectMany(notice => new[] { notice.AuthorId })
                .Where(id => id != Guid.Empty)
                .Distinct();

            var userProfiles = await _profileService.ResolveAsync(userIds, cancellationToken);

            if (userProfiles.Count > 0)
            {
                foreach (var notice in boardNoticesDto)
                {
                    var isExist = userProfiles.TryGetValue(notice.AuthorId, out var profile);

                    if (isExist && profile is not null)
                    {
                        notice.AuthorName = profile.Username;
                    }
                }
            }

            return Result.Success(boardNoticesDto.ToPaginatedList(request.PageIndex, request.PageSize, count));
        }
    }
}
