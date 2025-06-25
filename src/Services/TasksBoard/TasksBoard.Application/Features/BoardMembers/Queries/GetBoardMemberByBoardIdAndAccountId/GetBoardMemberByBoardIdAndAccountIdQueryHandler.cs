using AutoMapper;
using Common.Blocks.Exceptions;
using MediatR;
using Microsoft.Extensions.Logging;
using TasksBoard.Application.DTOs;
using TasksBoard.Domain.Entities;
using TasksBoard.Domain.Interfaces.UnitOfWorks;

namespace TasksBoard.Application.Features.BoardMembers.Queries.GetBoardMemberByBoardIdAndAccountId
{
    public class GetBoardMemberByBoardIdAndAccountIdQueryHandler(
        IUnitOfWork unitOfWork,
        ILogger<GetBoardMemberByBoardIdAndAccountIdQueryHandler> logger,
        IMapper mapper) : IRequestHandler<GetBoardMemberByBoardIdAndAccountIdQuery, BoardMemberDto>
    {
        private readonly IUnitOfWork _unitOfWork = unitOfWork;
        private readonly ILogger<GetBoardMemberByBoardIdAndAccountIdQueryHandler> _logger = logger;
        private readonly IMapper _mapper = mapper;

        public async Task<BoardMemberDto> Handle(GetBoardMemberByBoardIdAndAccountIdQuery request, CancellationToken cancellationToken)
        {
            var boardExist = await _unitOfWork.GetRepository<Board>().ExistAsync(request.BoardId, cancellationToken);
            if (!boardExist)
            {
                _logger.LogWarning($"Board with id '{request.BoardId}' not found.");
                throw new NotFoundException($"Board with id '{request.BoardId}' not found.");
            }

            var boardMember = await _unitOfWork.GetBoardMemberRepository().GetByBoardIdAndAccountIdAsync(request.BoardId, request.AccountId, cancellationToken);
            if (boardMember is null)
            {
                _logger.LogWarning($"Board member with account id '{request.AccountId} in board with id '{request.BoardId} not found.");
                throw new NotFoundException($"Board member with account id '{request.AccountId} in board with id '{request.BoardId} not found.");
            }

            var boardMemberDto = _mapper.Map<BoardMemberDto>(boardMember);

            return boardMemberDto;
        }
    }
}
