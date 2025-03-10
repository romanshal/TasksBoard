using AutoMapper;
using MediatR;
using Microsoft.Extensions.Logging;
using TasksBoard.Application.DTOs;
using TasksBoard.Domain.Entities;
using TasksBoard.Domain.Interfaces.UnitOfWorks;

namespace TasksBoard.Application.Features.BoardNoticeStatuses.Queries.GetBoardNoticeStatuses
{
    public class GetBoardNoticeStatusesQueryHandler(
        IUnitOfWork unitOfWork,
        ILogger<GetBoardNoticeStatusesQueryHandler> logger,
        IMapper mapper) : IRequestHandler<GetBoardNoticeStatusesQuery, IEnumerable<BoardNoticeStatusDto>>
    {
        private readonly IUnitOfWork _unitOfWork = unitOfWork;
        private readonly ILogger<GetBoardNoticeStatusesQueryHandler> _logger = logger;
        private readonly IMapper _mapper = mapper;

        public async Task<IEnumerable<BoardNoticeStatusDto>> Handle(GetBoardNoticeStatusesQuery request, CancellationToken cancellationToken)
        {
            var noticeStatuses = await _unitOfWork.GetRepository<BoardNoticeStatus>().GetAllAsync(cancellationToken);

            var noticeStatusesDto = _mapper.Map<IEnumerable<BoardNoticeStatusDto>>(noticeStatuses);

            return noticeStatusesDto;
        }
    }
}
