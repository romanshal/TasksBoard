using Authentication.Application.Dtos;
using Authentication.Domain.Entities;
using AutoMapper;
using Common.Blocks.Exceptions;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;

namespace Authentication.Application.Features.Manage.Queries.GetUserInfo
{
    public class GetUserInfoQueryHandler(
         UserManager<ApplicationUser> userManager,
         IMapper mapper,
         ILogger<GetUserInfoQueryHandler> logger) : IRequestHandler<GetUserInfoQuery, UserInfoDto>
    {
        private readonly UserManager<ApplicationUser> _userManager = userManager;
        private readonly IMapper _mapper = mapper;
        private readonly ILogger<GetUserInfoQueryHandler> _logger = logger;

        public async Task<UserInfoDto> Handle(GetUserInfoQuery request, CancellationToken cancellationToken)
        {
            var user = await _userManager.FindByIdAsync(request.UserId.ToString());
            if (user is null)
            {
                _logger.LogWarning($"User with id {request.UserId} not found.");
                throw new NotFoundException($"User with id {request.UserId} not found.");
            }

            var userDto = _mapper.Map<UserInfoDto>(user);

            return userDto;
        }
    }
}
