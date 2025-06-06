using AutoMapper;
using MediatR;
using Microsoft.Extensions.Logging;
using TasksBoard.Application.DTOs;
using TasksBoard.Domain.Interfaces.UnitOfWorks;

namespace TasksBoard.Application.Features.BoardInvites.Queries.GetBoardInviteRequestByToAccountId
{
    public class GetBoardInviteRequestByToAccountIdQueryHandler(
        IUnitOfWork unitOfWork,
        ILogger<GetBoardInviteRequestByToAccountIdQueryHandler> logger,
        IMapper mapper) : IRequestHandler<GetBoardInviteRequestByToAccountIdQuery, IEnumerable<BoardInviteRequestDto>>
    {
        private readonly IUnitOfWork _unitOfWork = unitOfWork;
        private readonly ILogger<GetBoardInviteRequestByToAccountIdQueryHandler> _logger = logger;
        private readonly IMapper _mapper = mapper;

        public async Task<IEnumerable<BoardInviteRequestDto>> Handle(GetBoardInviteRequestByToAccountIdQuery request, CancellationToken cancellationToken)
        {
            var inviteRequests = await _unitOfWork.GetBoardInviteRequestRepository().GetByToAccountIdAsync(request.AccountId, cancellationToken);

            var inviteRequestsDto = _mapper.Map<IEnumerable<BoardInviteRequestDto>>(inviteRequests);

            return inviteRequestsDto;
        }
    }
}
