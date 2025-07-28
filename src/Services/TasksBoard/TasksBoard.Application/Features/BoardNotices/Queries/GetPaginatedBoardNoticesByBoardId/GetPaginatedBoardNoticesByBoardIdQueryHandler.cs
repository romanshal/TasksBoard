using AutoMapper;
using Common.Blocks.Exceptions;
using Common.Blocks.Extensions;
using Common.Blocks.Models;
using Common.Blocks.Models.DomainResults;
using MediatR;
using Microsoft.Extensions.Logging;
using TasksBoard.Application.DTOs;
using TasksBoard.Application.Interfaces.UnitOfWorks;
using TasksBoard.Domain.Constants.Errors.DomainErrors;
using TasksBoard.Domain.Entities;

namespace TasksBoard.Application.Features.BoardNotices.Queries.GetPaginatedBoardNoticesByBoardId
{
    public class GetPaginatedBoardNoticesByBoardIdQueryHandler(
        IUnitOfWork unitOfWork,
        ILogger<GetPaginatedBoardNoticesByBoardIdQueryHandler> logger,
        IMapper mapper) : IRequestHandler<GetPaginatedBoardNoticesByBoardIdQuery, Result<PaginatedList<BoardNoticeDto>>>
    {
        private readonly IUnitOfWork _unitOfWork = unitOfWork;
        private readonly ILogger<GetPaginatedBoardNoticesByBoardIdQueryHandler> _logger = logger;
        private readonly IMapper _mapper = mapper;

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

            var boardNotice = await _unitOfWork.GetBoardNoticeRepository().GetPaginatedByBoardIdAsync(request.BoardId, request.PageIndex, request.PageSize, cancellationToken);

            var boardNoticeDto = _mapper.Map<IEnumerable<BoardNoticeDto>>(boardNotice);

            return Result.Success(boardNoticeDto.ToPaginatedList(request.PageIndex, request.PageSize, count));
        }
    }
}
