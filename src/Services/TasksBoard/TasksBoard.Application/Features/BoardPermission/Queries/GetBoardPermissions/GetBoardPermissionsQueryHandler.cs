using AutoMapper;
using MediatR;
using Microsoft.Extensions.Logging;
using TasksBoard.Application.DTOs;
using TasksBoard.Application.Interfaces.UnitOfWorks;

namespace TasksBoard.Application.Features.BoardPermission.Queries.GetBoardPermissions
{
    public class GetBoardPermissionsQueryHandler(
        ILogger<GetBoardPermissionsQueryHandler> logger,
        IUnitOfWork unitOfWork,
        IMapper mapper) : IRequestHandler<GetBoardPermissionsQuery, IEnumerable<BoardPermissionDto>>
    {
        private readonly ILogger<GetBoardPermissionsQueryHandler> _logger = logger;
        private readonly IUnitOfWork _unitOfWork = unitOfWork;
        private readonly IMapper _mapper = mapper;

        public async Task<IEnumerable<BoardPermissionDto>> Handle(GetBoardPermissionsQuery request, CancellationToken cancellationToken)
        {
            var permissions = await _unitOfWork.GetRepository<Domain.Entities.BoardPermission>().GetAllAsync(cancellationToken);

            var permissionsDto = _mapper.Map<IEnumerable<BoardPermissionDto>>(permissions);

            return permissionsDto.OrderBy(permission => permission.AccessLevel);
        }
    }
}
