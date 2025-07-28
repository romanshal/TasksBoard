using AutoMapper;
using Chat.Application.DTOs;
using Chat.Domain.Interfaces.UnitOfWorks;
using Common.Blocks.Models.DomainResults;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Chat.Application.Features.BoardMessages.Queries.GetBoardMessagesByBoardId
{
    public class GetBoardMessagesByBoardIdQueryHandler(
        IUnitOfWork unitOfWork,
        ILogger<GetBoardMessagesByBoardIdQueryHandler> logger,
        IMapper mapper) : IRequestHandler<GetBoardMessagesByBoardIdQuery, Result<IEnumerable<BoardMessageDto>>>
    {
        private readonly IUnitOfWork _unitOfWork = unitOfWork;
        private readonly ILogger<GetBoardMessagesByBoardIdQueryHandler> _logger = logger;
        private readonly IMapper _mapper = mapper;

        public async Task<Result<IEnumerable<BoardMessageDto>>> Handle(GetBoardMessagesByBoardIdQuery request, CancellationToken cancellationToken)
        {
            var boardMessages = await _unitOfWork.GetBoardMessagesRepository().GetPaginatedByBoardIdAsync(request.BoardId, request.PageIndex, request.PageSize, cancellationToken);

            var boardMessageDto = _mapper.Map<IEnumerable<BoardMessageDto>>(boardMessages);

            return Result.Success(boardMessageDto);
        }
    }
}
