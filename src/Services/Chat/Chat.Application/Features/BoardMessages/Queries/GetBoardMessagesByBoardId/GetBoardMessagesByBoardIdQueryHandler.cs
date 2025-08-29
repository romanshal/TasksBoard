using AutoMapper;
using Chat.Application.DTOs;
using Chat.Domain.Interfaces.UnitOfWorks;
using Common.Blocks.Models.DomainResults;
using Common.gRPC.Interfaces.Services;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Chat.Application.Features.BoardMessages.Queries.GetBoardMessagesByBoardId
{
    public class GetBoardMessagesByBoardIdQueryHandler(
        IUnitOfWork unitOfWork,
        ILogger<GetBoardMessagesByBoardIdQueryHandler> logger,
        IMapper mapper,
        IUserProfileService profileService) : IRequestHandler<GetBoardMessagesByBoardIdQuery, Result<IEnumerable<BoardMessageDto>>>
    {
        private readonly IUnitOfWork _unitOfWork = unitOfWork;
        private readonly ILogger<GetBoardMessagesByBoardIdQueryHandler> _logger = logger;
        private readonly IMapper _mapper = mapper;
        private readonly IUserProfileService _profileService = profileService;

        public async Task<Result<IEnumerable<BoardMessageDto>>> Handle(GetBoardMessagesByBoardIdQuery request, CancellationToken cancellationToken)
        {
            var boardMessages = await _unitOfWork.GetBoardMessagesRepository().GetPaginatedByBoardIdAsync(request.BoardId, request.PageIndex, request.PageSize, cancellationToken);

            var boardMessageDto = _mapper.Map<IEnumerable<BoardMessageDto>>(boardMessages);

            var userIds = boardMessageDto
                .SelectMany(req => new[] { req.AccountId })
                .Where(id => id != Guid.Empty)
                .ToHashSet();

            var userProfiles = await _profileService.ResolveAsync(userIds, cancellationToken);

            if (userProfiles.Count > 0)
            {
                foreach (var message in boardMessageDto)
                {
                    var isExist = userProfiles.TryGetValue(message.AccountId, out var profile);

                    if (isExist && profile is not null)
                    {
                        message.MemberNickname = profile.Username;
                    }
                }
            }

            return Result.Success(boardMessageDto);
        }
    }
}
