using AutoMapper;
using Common.Blocks.Extensions;
using Common.Blocks.Models;
using Common.Blocks.Models.DomainResults;
using MediatR;
using Microsoft.Extensions.Logging;
using TasksBoard.Application.DTOs;
using TasksBoard.Application.Models;
using TasksBoard.Domain.Entities;
using TasksBoard.Domain.Interfaces.Services;
using TasksBoard.Domain.Interfaces.UnitOfWorks;

namespace TasksBoard.Application.Features.BoardNotices.Queries.GetPaginatedBoardNotices
{
    public class GetPaginatedBoardNoticesQueryHandler(
        IUnitOfWork unitOfWork,
        ILogger<GetPaginatedBoardNoticesQueryHandler> logger,
        IMapper mapper,
        IUserProfileService profileService) : IRequestHandler<GetPaginatedListQuery<BoardNoticeDto>, Result<PaginatedList<BoardNoticeDto>>>
    {
        private readonly IUnitOfWork _unitOfWork = unitOfWork;
        private readonly ILogger<GetPaginatedBoardNoticesQueryHandler> _logger = logger;
        private readonly IMapper _mapper = mapper;
        private readonly IUserProfileService _profileService = profileService;

        public async Task<Result<PaginatedList<BoardNoticeDto>>> Handle(GetPaginatedListQuery<BoardNoticeDto> request, CancellationToken cancellationToken)
        {
            var count = await _unitOfWork.GetRepository<BoardNotice>().CountAsync(cancellationToken);
            if (count == 0)
            {
                _logger.LogInformation("No board notices entities in database.");
                return Result.Success(new PaginatedList<BoardNoticeDto>([], request.PageIndex, request.PageSize));
            }

            var boardNotice = await _unitOfWork.GetRepository<BoardNotice>().GetPaginatedAsync(request.PageIndex, request.PageSize, cancellationToken);

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
