using AutoMapper;
using Common.Blocks.Exceptions;
using MediatR;
using Microsoft.Extensions.Logging;
using TasksBoard.Application.DTOs;
using TasksBoard.Domain.Entities;
using TasksBoard.Domain.Interfaces.UnitOfWorks;

namespace TasksBoard.Application.Features.BoardNotices.Queries.GetBoardNoticeById
{
    public class GetBoardNoticeByIdQueryHandler(
        ILogger<GetBoardNoticeByIdQueryHandler> logger,
        IUnitOfWork unitOfWork,
        IMapper mapper) : IRequestHandler<GetBoardNoticeByIdQuery, BoardNoticeDto>
    {
        private readonly ILogger<GetBoardNoticeByIdQueryHandler> _logger = logger;
        private readonly IUnitOfWork _unitOfWork = unitOfWork;
        private readonly IMapper _mapper = mapper;

        public async Task<BoardNoticeDto> Handle(GetBoardNoticeByIdQuery request, CancellationToken cancellationToken)
        {
            var boardNotice = await _unitOfWork.GetRepository<BoardNotice>().GetAsync(request.Id, cancellationToken);
            if (boardNotice is null)
            {
                _logger.LogWarning($"Board notice with id '{request.Id}' was not found.");
                throw new NotFoundException($"Board notice with id '{request.Id}' was not found.");
            }
            var boardNoticeDto = _mapper.Map<BoardNoticeDto>(boardNotice);

            return boardNoticeDto;
        }
    }
}
