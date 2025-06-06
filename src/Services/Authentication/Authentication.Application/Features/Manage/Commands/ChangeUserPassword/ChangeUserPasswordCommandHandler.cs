using Authentication.Application.Features.Manage.Queries.GetUserInfo;
using Authentication.Domain.Entities;
using AutoMapper;
using Common.Blocks.Exceptions;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace Authentication.Application.Features.Manage.Commands.ChangeUserPassword
{
    public class ChangeUserPasswordCommandHandler(
        UserManager<ApplicationUser> userManager,
         IMapper mapper,
         ILogger<ChangeUserPasswordCommandHandler> logger) : IRequestHandler<ChangeUserPasswordCommand, Guid>
    {
        private readonly UserManager<ApplicationUser> _userManager = userManager;
        private readonly IMapper _mapper = mapper;
        private readonly ILogger<ChangeUserPasswordCommandHandler> _logger = logger;

        public async Task<Guid> Handle(ChangeUserPasswordCommand request, CancellationToken cancellationToken)
        {
            var user = await _userManager.FindByIdAsync(request.UserId.ToString());
            if (user is null)
            {
                _logger.LogWarning($"User with id {request.UserId} not found.");
                throw new NotFoundException($"User with id {request.UserId} not found.");
            }

            var validPassword = await _userManager.CheckPasswordAsync(user, request.CurrentPassword);
            if(!validPassword)
            {
                _logger.LogWarning($"Invalid password for user: {request.UserId}.");
                throw new InvalidPasswordException($"Invalid password for user");
            }

            var result = await _userManager.ChangePasswordAsync(user, request.CurrentPassword, request.NewPassword);
            if (!result.Succeeded)
            {
                _logger.LogCritical($"Can't update user password with id: {user.Id}. Errors: {string.Join("; ", result.Errors)}.");
                throw new Exception($"Can't update user password with id: {user.Id}. Errors: {string.Join("; ", result.Errors)}.");
            }

            _logger.LogInformation($"Password for user with id '{user.Id}' was successfully updated.");

            return user.Id;
        }
    }
}
