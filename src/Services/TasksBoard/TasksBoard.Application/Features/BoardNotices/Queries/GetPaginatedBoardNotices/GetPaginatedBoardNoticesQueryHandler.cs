using AutoMapper;
using Common.Blocks.Extensions;
using Common.Blocks.Models;
using Common.Blocks.Models.DomainResults;
using MediatR;
using Microsoft.Extensions.Logging;
using TasksBoard.Application.DTOs;
using TasksBoard.Application.Handlers;
using TasksBoard.Application.Models;
using TasksBoard.Domain.Entities;
using TasksBoard.Domain.Interfaces.UnitOfWorks;

namespace TasksBoard.Application.Features.BoardNotices.Queries.GetPaginatedBoardNotices
{
    public class GetPaginatedBoardNoticesQueryHandler(
        IUnitOfWork unitOfWork,
        ILogger<GetPaginatedBoardNoticesQueryHandler> logger,
        IMapper mapper,
        UserProfileHandler profileHandler) : IRequestHandler<GetPaginatedListQuery<BoardNoticeDto>, Result<PaginatedList<BoardNoticeDto>>>
    {
        private readonly IUnitOfWork _unitOfWork = unitOfWork;
        private readonly ILogger<GetPaginatedBoardNoticesQueryHandler> _logger = logger;
        private readonly IMapper _mapper = mapper;
        private readonly UserProfileHandler _profileHandler = profileHandler;

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

            await _profileHandler.Handle(
                boardNoticesDto,
                x => x.AuthorId,
                (x, username, _) => x.AuthorName = username,
                cancellationToken);

            return Result.Success(boardNoticesDto.ToPaginatedList(request.PageIndex, request.PageSize, count));
        }
    }
}
