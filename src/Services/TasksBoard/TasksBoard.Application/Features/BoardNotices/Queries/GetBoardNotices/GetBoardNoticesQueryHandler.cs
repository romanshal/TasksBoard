using AutoMapper;
using MediatR;
using Microsoft.Extensions.Logging;
using TasksBoard.Application.DTOs;
using TasksBoard.Domain.Entities;
using TasksBoard.Domain.Interfaces.UnitOfWorks;

namespace TasksBoard.Application.Features.BoardNotices.Queries.GetBoardNotices
{
    public class GetBoardNoticesQueryHandler(
        IUnitOfWork unitOfWork,
        ILogger<GetBoardNoticesQueryHandler> logger,
        IMapper mapper) : IRequestHandler<GetBoardNoticesQuery, IEnumerable<BoardNoticeDto>>
    {
        private readonly IUnitOfWork _unitOfWork = unitOfWork;
        private readonly ILogger<GetBoardNoticesQueryHandler> _logger = logger;
        private readonly IMapper _mapper = mapper;

        public async Task<IEnumerable<BoardNoticeDto>> Handle(GetBoardNoticesQuery request, CancellationToken cancellationToken)
        {
            var boardNotices = await _unitOfWork.GetRepository<BoardNotice>().GetAllAsync(cancellationToken);

            var boardNoticesDto = _mapper.Map<IEnumerable<BoardNoticeDto>>(boardNotices);

            return boardNoticesDto;
        }
    }
}
