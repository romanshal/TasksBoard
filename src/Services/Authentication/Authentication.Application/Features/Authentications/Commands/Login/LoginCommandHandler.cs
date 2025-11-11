using Authentication.Application.Dtos;
using Authentication.Application.Handlers;
using Authentication.Domain.Constants.AuthenticationErrors;
using Authentication.Domain.Entities;
using Common.Blocks.Models.DomainResults;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;

namespace Authentication.Application.Features.Authentications.Commands.Login
{
    public class LoginCommandHandler(
        UserManager<ApplicationUser> userManager,
        SignInManager<ApplicationUser> signInManager,
        SignInHandler signInHandler,
        ILogger<LoginCommandHandler> logger) : IRequestHandler<LoginCommand, Result<AuthenticationDto>>
    {
        private readonly UserManager<ApplicationUser> _userManager = userManager;
        private readonly SignInManager<ApplicationUser> _signInManager = signInManager;
        private readonly SignInHandler _signInHandler = signInHandler;
        private readonly ILogger<LoginCommandHandler> _logger = logger;

        public async Task<Result<AuthenticationDto>> Handle(LoginCommand request, CancellationToken cancellationToken)
        {
            ApplicationUser? user;
            if (request.UsernameOrEmail.Contains('@'))
                user = await _userManager.FindByEmailAsync(request.UsernameOrEmail);
            else
                user = await _userManager.FindByNameAsync(request.UsernameOrEmail);

            if (user is null)
            {
                return Result.Failure<AuthenticationDto>(AuthenticationErrors.UserNotFound(request.UsernameOrEmail));
            }
            //else if (!user.EmailConfirmed)
            //{
            //    return Result.Failure<AuthenticationDto>(AuthenticationErrors.EmailNotConfirmed);
            //}
            else if (await _userManager.IsLockedOutAsync(user))
            {
                _logger.LogWarning("Signin is locked for user: {username}.", request.UsernameOrEmail);
                return Result.Failure<AuthenticationDto>(AuthenticationErrors.Locked);
            }
            else if (user.TwoFactorEnabled)
            {
                return new AuthenticationDto
                {
                    TwoFactorEnabled = true,
                    AccessToken = null,
                    AccessTokenExpiredAt = null,
                    RefreshToken = null,
                    RefreshTokenExpiredAt = null,
                    UserId = user.Id,
                    DeviceId = null
                };
            }

            var signInResult = await _signInManager.PasswordSignInAsync(user.UserName!, request.Password, false, false);
            if (!signInResult.Succeeded)
            {
                if (signInResult.IsNotAllowed)
                {
                    _logger.LogWarning("Signin is not allowed for user {username}.", user.UserName);
                    return Result.Failure<AuthenticationDto>(AuthenticationErrors.NotAllowed);
                }

                _logger.LogWarning("Signin faulted for user: {username}.", user.UserName);
                return Result.Failure<AuthenticationDto>(AuthenticationErrors.Invalid);
            }

            //await _signInManager.SignInAsync(user, false, "Password");

            return await _signInHandler.HandleAsync(user, request.UserAgent, request.UserIp, cancellationToken);
        }
    }
}
