using Authentication.Application.Dtos;
using Authentication.Domain.Constants.ManageErrors;
using Authentication.Domain.Entities;
using AutoMapper;
using Common.Blocks.Models.DomainResults;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;

namespace Authentication.Application.Features.Manage.Queries.GetUserInfo
{
    public class GetUserInfoQueryHandler(
         UserManager<ApplicationUser> userManager,
         IMapper mapper,
         ILogger<GetUserInfoQueryHandler> logger) : IRequestHandler<GetUserInfoQuery, Result<UserInfoDto>>
    {
        private readonly UserManager<ApplicationUser> _userManager = userManager;
        private readonly IMapper _mapper = mapper;
        private readonly ILogger<GetUserInfoQueryHandler> _logger = logger;

        public async Task<Result<UserInfoDto>> Handle(GetUserInfoQuery request, CancellationToken cancellationToken)
        {
            var user = await _userManager.FindByIdAsync(request.Id.ToString());
            if (user is null)
            {
                _logger.LogWarning("User with id '{userId}' not found.", request.Id);
                return Result.Failure<UserInfoDto>(ManageErrors.UserNotFound);
            }

            var userDto = _mapper.Map<UserInfoDto>(user);

            return Result.Success(userDto);
        }
    }
}
