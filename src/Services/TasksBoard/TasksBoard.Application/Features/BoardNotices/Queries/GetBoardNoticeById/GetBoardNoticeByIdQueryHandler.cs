using AutoMapper;
using Common.Blocks.Models.DomainResults;
using MediatR;
using Microsoft.Extensions.Logging;
using TasksBoard.Application.DTOs;
using TasksBoard.Application.Interfaces.UnitOfWorks;
using TasksBoard.Domain.Constants.Errors.DomainErrors;
using TasksBoard.Domain.Entities;

namespace TasksBoard.Application.Features.BoardNotices.Queries.GetBoardNoticeById
{
    public class GetBoardNoticeByIdQueryHandler(
        ILogger<GetBoardNoticeByIdQueryHandler> logger,
        IUnitOfWork unitOfWork,
        IMapper mapper) : IRequestHandler<GetBoardNoticeByIdQuery, Result<BoardNoticeDto>>
    {
        private readonly ILogger<GetBoardNoticeByIdQueryHandler> _logger = logger;
        private readonly IUnitOfWork _unitOfWork = unitOfWork;
        private readonly IMapper _mapper = mapper;

        public async Task<Result<BoardNoticeDto>> Handle(GetBoardNoticeByIdQuery request, CancellationToken cancellationToken)
        {
            var boardNotice = await _unitOfWork.GetRepository<BoardNotice>().GetAsync(request.Id, cancellationToken);
            if (boardNotice is null)
            {
                _logger.LogWarning("Board notice with id '{id}' was not found.", request.Id);
                return Result.Failure<BoardNoticeDto>(BoardErrors.NotFound);

                //throw new NotFoundException($"Board notice with id '{request.Id}' was not found.");
            }
            var boardNoticeDto = _mapper.Map<BoardNoticeDto>(boardNotice);

            return Result.Success(boardNoticeDto);
        }
    }
}
