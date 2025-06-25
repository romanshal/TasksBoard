using Authentication.Application.Dtos;
using Authentication.Domain.Interfaces.UnitOfWorks;
using AutoMapper;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Authentication.Application.Features.Manage.Queries.GetUserImage
{
    public class GetUserImageQueryHandler(
        IUnitOfWork unitOfWork,
        IMapper mapper,
        ILogger<GetUserImageQueryHandler> logger) : IRequestHandler<GetUserImageQuery, UserImageDto>
    {
        private readonly IUnitOfWork _unitOfWork = unitOfWork;
        private readonly IMapper _mapper = mapper;
        private readonly ILogger<GetUserImageQueryHandler> _logger = logger;

        public async Task<UserImageDto> Handle(GetUserImageQuery request, CancellationToken cancellationToken)
        {
            var userImage = await _unitOfWork.GetApplicationUserImageRepository().GetByUserIdAsync(request.UserId, cancellationToken);

            var userImageDto = _mapper.Map<UserImageDto>(userImage);

            return userImageDto;
        }
    }
}
