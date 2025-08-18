using AutoMapper;
using Common.Blocks.Extensions;
using Common.Blocks.Interfaces.Services;
using Common.Blocks.Models;
using Common.Blocks.Models.DomainResults;
using MediatR;
using Microsoft.Extensions.Logging;
using TasksBoard.Application.DTOs;
using TasksBoard.Domain.Constants.Errors.DomainErrors;
using TasksBoard.Domain.Entities;
using TasksBoard.Domain.Interfaces.UnitOfWorks;

namespace TasksBoard.Application.Features.BoardNotices.Queries.GetPaginatedBoardNoticesByBoardId
{
    public class GetPaginatedBoardNoticesByBoardIdQueryHandler(
        IUnitOfWork unitOfWork,
        ILogger<GetPaginatedBoardNoticesByBoardIdQueryHandler> logger,
        IMapper mapper,
        IUserProfileService profileService) : IRequestHandler<GetPaginatedBoardNoticesByBoardIdQuery, Result<PaginatedList<BoardNoticeDto>>>
    {
        private readonly IUnitOfWork _unitOfWork = unitOfWork;
        private readonly ILogger<GetPaginatedBoardNoticesByBoardIdQueryHandler> _logger = logger;
        private readonly IMapper _mapper = mapper;
        private readonly IUserProfileService _profileService = profileService;

        public async Task<Result<PaginatedList<BoardNoticeDto>>> Handle(GetPaginatedBoardNoticesByBoardIdQuery request, CancellationToken cancellationToken)
        {
            var boardExist = await _unitOfWork.GetRepository<Board>().ExistAsync(request.BoardId, cancellationToken);
            if (!boardExist)
            {
                _logger.LogWarning("Board with id '{boardId}' not found.", request.BoardId);
                return Result.Failure<PaginatedList<BoardNoticeDto>>(BoardErrors.NotFound);

                //throw new NotFoundException($"Board with id '{request.BoardId}' not found.");
            }

            var count = await _unitOfWork.GetBoardNoticeRepository().CountByBoardIdAsync(request.BoardId, cancellationToken);
            if (count == 0)
            {
                _logger.LogInformation("No board notices entities in database.");
                return Result.Success(new PaginatedList<BoardNoticeDto>([], request.PageIndex, request.PageSize));
            }

            var boardNotices = await _unitOfWork.GetBoardNoticeRepository().GetPaginatedByBoardIdAsync(request.BoardId, request.PageIndex, request.PageSize, cancellationToken);

            var userIds = boardNotices
                .SelectMany(notice => new[] { notice.AuthorId })
                .Where(id => id != Guid.Empty)
                .Distinct();

            var boardNoticeDto = _mapper.Map<IEnumerable<BoardNoticeDto>>(boardNotices);

            var userProfiles = await _profileService.ResolveAsync(userIds, cancellationToken);

            if (userProfiles.Count > 0)
            {
                foreach (var notice in boardNoticeDto)
                {
                    var isExist = userProfiles.TryGetValue(notice.AuthorId, out var profile);

                    if (isExist && profile is not null)
                    {
                        notice.AuthorName = profile.Username;
                    }
                }
            }

            return Result.Success(boardNoticeDto.ToPaginatedList(request.PageIndex, request.PageSize, count));
        }
    }
}
