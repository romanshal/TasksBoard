using Authentication.Domain.Constants.ManageErrors;
using Authentication.Domain.Entities;
using Common.Blocks.Models.DomainResults;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;

namespace Authentication.Application.Features.Manage.Commands.ChangeUserPassword
{
    public class ChangeUserPasswordCommandHandler(
        UserManager<ApplicationUser> userManager,
         ILogger<ChangeUserPasswordCommandHandler> logger) : IRequestHandler<ChangeUserPasswordCommand, Result>
    {
        private readonly UserManager<ApplicationUser> _userManager = userManager;
        private readonly ILogger<ChangeUserPasswordCommandHandler> _logger = logger;

        public async Task<Result> Handle(ChangeUserPasswordCommand request, CancellationToken cancellationToken)
        {
            var user = await _userManager.FindByIdAsync(request.UserId.ToString());
            if (user is null)
            {
                _logger.LogWarning("User with id '{userId}' not found.", request.UserId);
                return Result.Failure(ManageErrors.UserNotFound);
            }

            var validPassword = await _userManager.CheckPasswordAsync(user, request.CurrentPassword);
            if (!validPassword)
            {
                _logger.LogWarning("Invalid password for user: {userId}.", request.UserId);
                return Result.Failure(ManageErrors.InvalidPassword);
            }

            var result = await _userManager.ChangePasswordAsync(user, request.CurrentPassword, request.NewPassword);
            if (!result.Succeeded)
            {
                _logger.LogCritical("Can't update user password with id: {id}. Errors: {errors}.", user.Id, string.Join("; ", result.Errors));
                return Result.Failure(ManageErrors.CantUpdatePassword);
            }

            _logger.LogInformation("Password for user with id '{Id}' was successfully updated.", user.Id);

            return Result.Success();
        }
    }
}
