using AutoMapper;
using Common.Blocks.Exceptions;
using MediatR;
using Microsoft.Extensions.Logging;
using TasksBoard.Application.DTOs;
using TasksBoard.Application.Interfaces.UnitOfWorks;
using TasksBoard.Domain.Entities;

namespace TasksBoard.Application.Features.ManageBoardMembers.Queries.GetBoardMemberPermissions
{
    public class GetBoardMemberPermissionsQueryHandler(
        IUnitOfWork unitOfWork,
        IMapper mapper,
        ILogger<GetBoardMemberPermissionsQueryHandler> logger) : IRequestHandler<GetBoardMemberPermissionsQuery, IEnumerable<BoardMemberPermissionDto>>
    {
        private readonly IUnitOfWork _unitOfWork = unitOfWork;
        private readonly IMapper _mapper = mapper;
        private readonly ILogger<GetBoardMemberPermissionsQueryHandler> _logger = logger;

        public async Task<IEnumerable<BoardMemberPermissionDto>> Handle(GetBoardMemberPermissionsQuery request, CancellationToken cancellationToken)
        {
            var board = await _unitOfWork.GetRepository<Board>().GetAsync(request.BoardId, cancellationToken);
            if (board is null)
            {
                _logger.LogWarning("Board with id '{boardId}' not found.", request.BoardId);
                throw new NotFoundException($"Board with id '{request.BoardId}' not found.");
            }

            var member = board.BoardMembers.FirstOrDefault(member => member.Id == request.MemberId);
            if (member is null)
            {
                _logger.LogWarning("Board member with id '{memberId}' not found in board '{boardId}'.", request.MemberId, request.BoardId);
                throw new NotFoundException($"Board member with id '{request.MemberId}' not found in board '{request.BoardId}'.");
            }

            var permissions = member.BoardMemberPermissions.ToList();

            var permissionsDto = _mapper.Map<IEnumerable<BoardMemberPermissionDto>>(permissions);

            return permissionsDto;
        }
    }
}
