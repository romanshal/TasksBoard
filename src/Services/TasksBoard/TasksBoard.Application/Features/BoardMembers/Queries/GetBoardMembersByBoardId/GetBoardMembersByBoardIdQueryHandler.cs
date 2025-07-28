using AutoMapper;
using Common.Blocks.Models.DomainResults;
using MediatR;
using Microsoft.Extensions.Logging;
using TasksBoard.Application.DTOs;
using TasksBoard.Application.Interfaces.UnitOfWorks;
using TasksBoard.Domain.Constants.Errors.DomainErrors;
using TasksBoard.Domain.Entities;

namespace TasksBoard.Application.Features.BoardMembers.Queries.GetBoardMembersByBoardId
{
    public class GetBoardMembersByBoardIdQueryHandler(
        IUnitOfWork unitOfWork,
        ILogger<GetBoardMembersByBoardIdQueryHandler> logger,
        IMapper mapper) : IRequestHandler<GetBoardMembersByBoardIdQuery, Result<IEnumerable<BoardMemberDto>>>
    {
        private readonly IUnitOfWork _unitOfWork = unitOfWork;
        private readonly ILogger<GetBoardMembersByBoardIdQueryHandler> _logger = logger;
        private readonly IMapper _mapper = mapper;

        public async Task<Result<IEnumerable<BoardMemberDto>>> Handle(GetBoardMembersByBoardIdQuery request, CancellationToken cancellationToken)
        {
            var boardExist = await _unitOfWork.GetRepository<Board>().ExistAsync(request.BoardId, cancellationToken);
            if (!boardExist)
            {
                _logger.LogWarning("Board with id '{boardId}' not found.", request.BoardId);
                return Result.Failure<IEnumerable<BoardMemberDto>>(BoardErrors.NotFound);

                //throw new NotFoundException($"Board with id '{request.BoardId}' not found.");
            }

            var boardMembers = await _unitOfWork.GetBoardMemberRepository().GetByBoardIdAsync(request.BoardId, cancellationToken);

            var boardMembersDto = _mapper.Map<IEnumerable<BoardMemberDto>>(boardMembers);

            return Result.Success(boardMembersDto);
        }
    }
}
