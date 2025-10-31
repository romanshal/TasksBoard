using Authentication.Application.Dtos;
using Authentication.Application.Handlers;
using Authentication.Domain.Constants.AuthenticationErrors;
using Authentication.Domain.Constants.TwoFactor;
using Authentication.Domain.Entities;
using Common.Blocks.Models.DomainResults;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;

namespace Authentication.Application.Features.Authentications.Commands.VerifyTwoFactor
{
    internal class VerifyTwoFactorCommandHandler(
        UserManager<ApplicationUser> userManager,
        SignInManager<ApplicationUser> signInManager,
        SignInHandler signInHandler,
        ILogger<VerifyTwoFactorCommandHandler> logger) : IRequestHandler<VerifyTwoFactorCommand, Result<AuthenticationDto>>
    {
        private readonly UserManager<ApplicationUser> _userManager = userManager;
        private readonly SignInManager<ApplicationUser> _signInManager = signInManager;
        private readonly SignInHandler _signInHandler = signInHandler;
        private readonly ILogger<VerifyTwoFactorCommandHandler> _logger = logger;

        public async Task<Result<AuthenticationDto>> Handle(VerifyTwoFactorCommand request, CancellationToken cancellationToken)
        {
            var user = await _userManager.FindByIdAsync(request.UserId.ToString());
            if (user is null)
            {
                return Result.Failure<AuthenticationDto>(AuthenticationErrors.UserNotFound());
            }

            SignInResult result;
            if (request.Provider == TokenOptions.DefaultAuthenticatorProvider)
            {
                result = await _signInManager.TwoFactorAuthenticatorSignInAsync(
                                        code: request.Code,
                                        isPersistent: false,
                                        rememberClient: request.RememberMachine);
            }
            else if (request.Provider == TokenOptions.DefaultPhoneProvider || request.Provider == TokenOptions.DefaultEmailProvider)
            {
                result = await _signInManager.TwoFactorSignInAsync(
                                        provider: request.Provider,
                                        code: request.Code,
                                        isPersistent: false,
                                        rememberClient: request.RememberMachine);
            }
            else
            {
                _logger.LogWarning("Invalid two factor provider from user with id '{}'.", request.UserId);
                return Result.Failure<AuthenticationDto>(TwoFactorErrors.InvalidProvider);
            }

            if (!result.Succeeded)
            {
                return Result.Failure<AuthenticationDto>(TwoFactorErrors.CantVerify);
            }

            return await _signInHandler.HandleAsync(user, request.UserAgent, request.UserIp, cancellationToken);
        }
    }
}
