using AutoMapper;
using Common.Blocks.Extensions;
using Common.Blocks.Models;
using Common.Blocks.Models.DomainResults;
using Common.gRPC.Interfaces.Services;
using MediatR;
using Microsoft.Extensions.Logging;
using TasksBoard.Application.DTOs;
using TasksBoard.Domain.Interfaces.UnitOfWorks;

namespace TasksBoard.Application.Features.BoardNotices.Queries.GetPaginatedBoardNoticesByUserId
{
    public class GetPaginatedBoardNoticesByUserIdQueryHandler(
        IUnitOfWork unitOfWork,
        ILogger<GetPaginatedBoardNoticesByUserIdQueryHandler> logger,
        IMapper mapper,
        IUserProfileService profileService) : IRequestHandler<GetPaginatedBoardNoticesByUserIdQuery, Result<PaginatedList<BoardNoticeDto>>>
    {
        private readonly IUnitOfWork _unitOfWork = unitOfWork;
        private readonly ILogger<GetPaginatedBoardNoticesByUserIdQueryHandler> _logger = logger;
        private readonly IMapper _mapper = mapper;
        private readonly IUserProfileService _profileService = profileService;

        public async Task<Result<PaginatedList<BoardNoticeDto>>> Handle(GetPaginatedBoardNoticesByUserIdQuery request, CancellationToken cancellationToken)
        {
            var count = await _unitOfWork.GetBoardNoticeRepository().CountByUserIdAsync(request.UserId, cancellationToken);
            if (count == 0)
            {
                _logger.LogInformation("No board notices entities in database.");
                return Result.Success(new PaginatedList<BoardNoticeDto>([], request.PageIndex, request.PageSize));
            }

            var boardNotice = await _unitOfWork.GetBoardNoticeRepository().GetPaginatedByUserIdAsync(request.UserId, request.PageIndex, request.PageSize, cancellationToken);

            var boardNoticesDto = _mapper.Map<IEnumerable<BoardNoticeDto>>(boardNotice);

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
