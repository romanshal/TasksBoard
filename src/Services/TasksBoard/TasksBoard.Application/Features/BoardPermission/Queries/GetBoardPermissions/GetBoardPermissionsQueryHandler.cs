using AutoMapper;
using Common.Blocks.Models.DomainResults;
using MediatR;
using Microsoft.Extensions.Logging;
using TasksBoard.Application.DTOs;
using TasksBoard.Domain.Interfaces.UnitOfWorks;

namespace TasksBoard.Application.Features.BoardPermission.Queries.GetBoardPermissions
{
    public class GetBoardPermissionsQueryHandler(
        ILogger<GetBoardPermissionsQueryHandler> logger,
        IUnitOfWork unitOfWork,
        IMapper mapper) : IRequestHandler<GetBoardPermissionsQuery, Result<IEnumerable<BoardPermissionDto>>>
    {
        private readonly ILogger<GetBoardPermissionsQueryHandler> _logger = logger;
        private readonly IUnitOfWork _unitOfWork = unitOfWork;
        private readonly IMapper _mapper = mapper;

        public async Task<Result<IEnumerable<BoardPermissionDto>>> Handle(GetBoardPermissionsQuery request, CancellationToken cancellationToken)
        {
            var permissions = await _unitOfWork.GetRepository<Domain.Entities.BoardPermission>().GetAllAsync(cancellationToken);

            var permissionsDto = _mapper.Map<IEnumerable<BoardPermissionDto>>(permissions.OrderBy(permission => permission.AccessLevel));

            return Result.Success(permissionsDto);
        }
    }
}
