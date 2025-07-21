using Authentication.Application.Dtos;
using Authentication.Application.Interfaces.Services;
using Authentication.Domain.Entities;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;

namespace Authentication.Application.Features.Authentications.Commands.ExternalLogin
{
    public class ExternalLoginCommandHandler(
        UserManager<ApplicationUser> userManager,
        SignInManager<ApplicationUser> signInManager,
        ITokenService tokenService,
        ILogger<ExternalLoginCommandHandler> logger) : IRequestHandler<ExternalLoginCommand, AuthenticationDto>
    {
        private readonly UserManager<ApplicationUser> _userManager = userManager;
        private readonly SignInManager<ApplicationUser> _signInManager = signInManager;
        private readonly ITokenService _tokenService = tokenService;
        private readonly ILogger<ExternalLoginCommandHandler> _logger = logger;

        public async Task<AuthenticationDto> Handle(ExternalLoginCommand request, CancellationToken cancellationToken)
        {
            var payload = await _tokenService.VerifyGoogleToken(request.Provider, request.TokenId);
            if (payload is null)
            {
                _logger.LogError("Invalid External Authentication.");
                throw new ArgumentNullException("Invalid External Authentication.");
            }

            var info = new UserLoginInfo(request.Provider!, payload.Subject, request.Provider);

            var user = await _userManager.FindByLoginAsync(info.LoginProvider, info.ProviderKey);
            if (user is null)
            {
                user = await _userManager.FindByEmailAsync(payload.Email);
                if (user is null)
                {
                    user = new ApplicationUser
                    {
                        Email = payload.Email,
                        UserName = payload.Email
                    };

                    var createResult = await _userManager.CreateAsync(user);
                    if (!createResult.Succeeded)
                    {
                        _logger.LogCritical("Can't create new user with username: {username}. Errors: {errors}.", user.UserName, string.Join("; ", createResult.Errors));
                        throw new Exception($"Can't create new user with username: {user.UserName}. Errors: {string.Join("; ", createResult.Errors)}.");
                    }

                    var addRoleResult = await _userManager.AddToRoleAsync(user, "user");
                    if (!addRoleResult.Succeeded)
                    {
                        _logger.LogCritical("Can't add role to user: {username}. Errors: {errors}.", user.UserName, string.Join("; ", addRoleResult.Errors));
                        throw new Exception($"Can't add role to user: {user.UserName}. Errors: {string.Join("; ", addRoleResult.Errors)}.");
                    }

                    var addLoginResult = await _userManager.AddLoginAsync(user, info);
                    if (!addLoginResult.Succeeded)
                    {
                        _logger.LogCritical("Can't add login to user: {username}. Errors: {errors}.", user.UserName, string.Join("; ", addLoginResult.Errors));
                        throw new Exception($"Can't add login to user: {user.UserName}. Errors: {string.Join("; ", addLoginResult.Errors)}.");
                    }
                }
                else
                {
                    var addLoginResult = await _userManager.AddLoginAsync(user, info);
                    if (!addLoginResult.Succeeded)
                    {
                        _logger.LogCritical("Can't add login to user: {username}. Errors: {errors}.", user.UserName, string.Join("; ", addLoginResult.Errors));
                        throw new Exception($"Can't add login to user: {user.UserName}. Errors: {string.Join("; ", addLoginResult.Errors)}.");
                    }
                }
            }

            if (user is null)
            {
                _logger.LogError("Invalid External Authentication.");
                throw new ArgumentNullException("Invalid External Authentication.");
            }

            await _signInManager.SignInAsync(user, false, request.Provider);

            var token = await _tokenService.GenerateTokenAsync(user);
            if (token is null || string.IsNullOrEmpty(token?.AccessToken) || string.IsNullOrEmpty(token?.RefreshToken))
            {
                _logger.LogCritical("Can't create access or refresh tokens for user {id}.", user.Id);
                throw new InvalidOperationException("Can't create access or refresh tokens.");
            }

            return new AuthenticationDto
            {
                AccessToken = token.AccessToken,
                RefreshToken = token.RefreshToken,
                UserId = user.Id
            };
        }
    }
}
