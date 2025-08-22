using Authentication.Application.Dtos;
using Authentication.Domain.Constants.AuthenticationErrors;
using Authentication.Domain.Entities;
using Authentication.Domain.Interfaces.Secutiry;
using Authentication.Domain.Models;
using Common.Blocks.Exceptions;
using Common.Blocks.Models;
using Common.Blocks.Models.DomainResults;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using System.Security.Claims;

namespace Authentication.Application.Features.Authentications.Commands.ExternalLoginCallback
{
    public class ExternalLoginCallbackCommandHandler(
        SignInManager<ApplicationUser> signInManager,
        UserManager<ApplicationUser> userManager,
        ITokenManager tokenService,
        ILogger<ExternalLoginCallbackCommandHandler> logger) : IRequestHandler<ExternalLoginCallbackCommand, Result<AuthenticationDto>>
    {
        private readonly SignInManager<ApplicationUser> _signInManager = signInManager;
        private readonly UserManager<ApplicationUser> _userManager = userManager;
        private readonly ITokenManager _tokenService = tokenService;
        private readonly ILogger<ExternalLoginCallbackCommandHandler> _logger = logger;

        public async Task<Result<AuthenticationDto>> Handle(ExternalLoginCallbackCommand request, CancellationToken cancellationToken)
        {
            var info = await _signInManager.GetExternalLoginInfoAsync();
            if (info == null)
            {
                return Result.Failure<AuthenticationDto>(ExternalAuthenticationErrors.LoginNotFound);
            }

            var provider = info.LoginProvider;
            var providerKey = info.ProviderKey;

            var email = info.Principal.FindFirstValue(ClaimTypes.Email);
            if (string.IsNullOrWhiteSpace(email))
            {
                return Result.Failure<AuthenticationDto>(ExternalAuthenticationErrors.Invalid("Provider did not return an email"));
            }

            var emailVerifiedClaim = info.Principal.FindFirst("email_verified")?.Value;
            if (emailVerifiedClaim is not null && (!bool.TryParse(emailVerifiedClaim, out var verified) || !verified))
            {
                return Result.Failure<AuthenticationDto>(ExternalAuthenticationErrors.Invalid("Email is not verified by provider"));
            }

            // Попытка входа по привязанному логину
            var signInResult = await _signInManager.ExternalLoginSignInAsync(provider, providerKey, isPersistent: false, bypassTwoFactor: false);

            ApplicationUser? user;

            if (signInResult.Succeeded)
            {
                user = await _userManager.FindByLoginAsync(provider, providerKey);
                if (user is null)
                {
                    return Result.Failure<AuthenticationDto>(ExternalAuthenticationErrors.UserNotFound);
                }
            }
            else
            {
                // Есть ли юзер с таким email?
                user = await _userManager.FindByEmailAsync(email);

                if (user == null)
                {
                    user = new ApplicationUser
                    {
                        UserName = email,
                        Email = email,
                        EmailConfirmed = emailVerifiedClaim is not null && string.Equals(emailVerifiedClaim, "true", StringComparison.OrdinalIgnoreCase)
                    };

                    var createResult = await _userManager.CreateAsync(user);
                    if (!createResult.Succeeded)
                    {
                        _logger.LogError("External login failed: {errors}", createResult.ToString());
                        return Result.Failure<AuthenticationDto>(ExternalAuthenticationErrors.CantCreate);
                    }
                }
                else
                {
                    // Если у пользователя уже есть локальный пароль или другой внешний логин,
                    // требуем сначала войти существующим способом и выполнить "link external login"
                    var logins = await _userManager.GetLoginsAsync(user);
                    var hasDifferentProviderLinked = logins.Any(l => !string.Equals(l.LoginProvider, provider, StringComparison.Ordinal));
                    var hasPassword = await _userManager.HasPasswordAsync(user);

                    if (hasPassword || hasDifferentProviderLinked)
                    {
                        return Result.Failure<AuthenticationDto>(ExternalAuthenticationErrors.AlreadyExist);
                    }
                }

                // Привязываем внешний логин к пользователю (AspNetUserLogins)
                var addLoginResult = await _userManager.AddLoginAsync(user, info);
                if (!addLoginResult.Succeeded)
                {
                    _logger.LogError("External login failed: {errors}", addLoginResult.ToString());
                    return Result.Failure<AuthenticationDto>(ExternalAuthenticationErrors.CantCreate);
                }
            }

            var generateModel = new GenerateTokensModel(user, request.DeviceId, request.UserAgent, request.UserIp);

            var token = await _tokenService.IssueAsync(generateModel, cancellationToken);
            if (token is null || string.IsNullOrEmpty(token?.AccessToken) || string.IsNullOrEmpty(token?.RefreshToken))
            {
                _logger.LogCritical("Can't create access or refresh tokens for user with id '{id}'.", user.Id);
                return Result.Failure<AuthenticationDto>(ExternalAuthenticationErrors.CantCreate);
            }

            _logger.LogInformation("Success external signin for user: {username}.", user.UserName);

            return new AuthenticationDto
            {
                AccessToken = token.AccessToken,
                RefreshToken = token.RefreshToken,
                UserId = user.Id
            };
        }
    }
}
