using Authentication.Application.Dtos;
using Authentication.Application.Handlers;
using Authentication.Domain.Constants.AuthenticationErrors;
using Authentication.Domain.Entities;
using Common.Blocks.Models.DomainResults;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using System.Security.Claims;

namespace Authentication.Application.Features.Authentications.Commands.ExternalLoginCallback
{
    internal class ExternalLoginCallbackCommandHandler(
        SignInManager<ApplicationUser> signInManager,
        UserManager<ApplicationUser> userManager,
        SignInHandler signInHandler,
        ILogger<ExternalLoginCallbackCommandHandler> logger) : IRequestHandler<ExternalLoginCallbackCommand, Result<AuthenticationDto>>
    {
        private readonly SignInManager<ApplicationUser> _signInManager = signInManager;
        private readonly UserManager<ApplicationUser> _userManager = userManager;
        private readonly SignInHandler _signInHandler = signInHandler;
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

            var name = info.Principal.FindFirstValue(ClaimTypes.GivenName) ??
                info.Principal.FindFirstValue(ClaimTypes.Surname) ??
                info.Principal.FindFirstValue(ClaimTypes.Name)?.Trim();

            var email = info.Principal.FindFirstValue(ClaimTypes.Email);

            if (string.IsNullOrWhiteSpace(email) || string.IsNullOrWhiteSpace(name))
            {
                _logger.LogError("External provider didn't return valid data.");
                return Result.Failure<AuthenticationDto>(ExternalAuthenticationErrors.Invalid("Invalid data."));
            }

            var emailVerifiedClaim = info.Principal.FindFirst("email_verified")?.Value;
            if (emailVerifiedClaim is not null && (!bool.TryParse(emailVerifiedClaim, out var verified) || !verified))
            {
                _logger.LogWarning("Email '{email}' isn't verified by provider '{provider}'.", email, provider);
                return Result.Failure<AuthenticationDto>(ExternalAuthenticationErrors.Invalid("Email isn't verified."));
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
                        UserName = name,
                        Email = email,
                        FirstName = info.Principal.FindFirstValue(ClaimTypes.GivenName),
                        Surname = info.Principal.FindFirstValue(ClaimTypes.Surname),
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

            return await _signInHandler.HandleAsync(user, request.UserAgent, request.UserIp, cancellationToken);
        }
    }
}
