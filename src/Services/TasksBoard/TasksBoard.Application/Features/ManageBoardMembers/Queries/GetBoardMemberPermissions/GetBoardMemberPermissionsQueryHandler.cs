using AutoMapper;
using Common.Blocks.Models.DomainResults;
using MediatR;
using Microsoft.Extensions.Logging;
using TasksBoard.Application.DTOs;
using TasksBoard.Domain.Constants.Errors.DomainErrors;
using TasksBoard.Domain.Entities;
using TasksBoard.Domain.Interfaces.UnitOfWorks;

namespace TasksBoard.Application.Features.ManageBoardMembers.Queries.GetBoardMemberPermissions
{
    public class GetBoardMemberPermissionsQueryHandler(
        IUnitOfWork unitOfWork,
        IMapper mapper,
        ILogger<GetBoardMemberPermissionsQueryHandler> logger) : IRequestHandler<GetBoardMemberPermissionsQuery, Result<IEnumerable<BoardMemberPermissionDto>>>
    {
        private readonly IUnitOfWork _unitOfWork = unitOfWork;
        private readonly IMapper _mapper = mapper;
        private readonly ILogger<GetBoardMemberPermissionsQueryHandler> _logger = logger;

        public async Task<Result<IEnumerable<BoardMemberPermissionDto>>> Handle(GetBoardMemberPermissionsQuery request, CancellationToken cancellationToken)
        {
            var board = await _unitOfWork.GetRepository<Board>().GetAsync(request.BoardId, cancellationToken);
            if (board is null)
            {
                _logger.LogWarning("Board with id '{boardId}' not found.", request.BoardId);
                Result.Failure<IEnumerable<BoardMemberPermissionDto>>(BoardErrors.NotFound);

                //throw new NotFoundException($"Board with id '{request.BoardId}' not found.");
            }

            var member = board!.BoardMembers.FirstOrDefault(member => member.Id == request.MemberId);
            if (member is null)
            {
                _logger.LogWarning("Board member with id '{memberId}' not found in board '{boardId}'.", request.MemberId, request.BoardId);
                return Result.Failure<IEnumerable<BoardMemberPermissionDto>>(BoardMemberErrors.NotFound);

                //throw new NotFoundException($"Board member with id '{request.MemberId}' not found in board '{request.BoardId}'.");
            }

            var permissions = member.BoardMemberPermissions.ToList();

            var permissionsDto = _mapper.Map<IEnumerable<BoardMemberPermissionDto>>(permissions);

            return Result.Success(permissionsDto);
        }
    }
}
