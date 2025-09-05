using Authentication.Application.Dtos;
using Authentication.Domain.Constants.ManageErrors;
using Authentication.Domain.Entities;
using AutoMapper;
using Common.Blocks.Models.DomainResults;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;

namespace Authentication.Application.Features.Manage.Commands.UpdateUserInfo
{
    public class UpdateUserInfoCommandHandler(
        UserManager<ApplicationUser> userManager,
        IMapper mapper,
        ILogger<UpdateUserInfoCommandHandler> logger) : IRequestHandler<UpdateUserInfoCommand, Result<UserInfoDto>>
    {
        private readonly UserManager<ApplicationUser> _userManager = userManager;
        private readonly IMapper _mapper = mapper;
        private readonly ILogger<UpdateUserInfoCommandHandler> _logger = logger;

        public async Task<Result<UserInfoDto>> Handle(UpdateUserInfoCommand request, CancellationToken cancellationToken)
        {
            var user = await _userManager.FindByIdAsync(request.UserId.ToString());
            if (user is null)
            {
                _logger.LogWarning("User with id '{userId}' not found.", request.UserId);
                return Result.Failure<UserInfoDto>(ManageErrors.UserNotFound);
            }

            user.UserName = request.Username;
            user.Email = request.Email;
            user.FirstName = request.Firstname;
            user.Surname = request.Surname;

            var result = await _userManager.UpdateAsync(user);
            if (!result.Succeeded)
            {
                _logger.LogCritical("Can't update user info with id: {id}. Errors: {errors}.", user.Id, string.Join("; ", result.Errors));
                return Result.Failure<UserInfoDto>(ManageErrors.CantUpdateInfo);
            }

            var userDto = _mapper.Map<UserInfoDto>(user);

            _logger.LogInformation("User with id '{id}' was successfully updated.", user.Id);

            return Result.Success(userDto);
        }
    }
}
