using Authentication.Application.Dtos;
using Authentication.Application.Models;
using Authentication.Domain.Entities;
using Authentication.Domain.Interfaces.Secutiry;
using Authentication.Domain.Models;
using Common.Blocks.Exceptions;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;

namespace Authentication.Application.Features.Authentications.Commands.Login
{
    public class LoginCommandHandler(
        UserManager<ApplicationUser> userManager,
        SignInManager<ApplicationUser> signInManager,
        ITokenManager tokenService,
        ILogger<LoginCommandHandler> logger) : IRequestHandler<LoginCommand, AuthenticationDto>
    {
        private readonly UserManager<ApplicationUser> _userManager = userManager;
        private readonly SignInManager<ApplicationUser> _signInManager = signInManager;
        private readonly ITokenManager _tokenService = tokenService;
        private readonly ILogger<LoginCommandHandler> _logger = logger;

        public async Task<AuthenticationDto> Handle(LoginCommand request, CancellationToken cancellationToken)
        {
            var user = await _userManager.FindByNameAsync(request.Username);
            if (user is null)
            {
                _logger.LogWarning("User with name {username} not found.", request.Username);
                throw new UnauthorizedException($"User with name {request.Username} not found.");
            }

            var signInResult = await _signInManager.PasswordSignInAsync(request.Username, request.Password, false, false);
            if (!signInResult.Succeeded)
            {
                if (signInResult.IsLockedOut)
                {
                    _logger.LogWarning("Signin is locked for user: {username}.", request.Username);
                    throw new LockedException($"Your account is temporarily locked. Please contact support for assistance.");
                }
                else if (signInResult.IsNotAllowed)
                {
                    _logger.LogWarning("Signin is not allowed for user {username}.", request.Username);
                    throw new NotAllowedException($"Signin is not allowed for user {request.Username}."); //TODO: change this later
                }

                _logger.LogWarning("Signin faulted for user: {username}.", request.Username);
                throw new SigninFaultedException($"The username or password you entered is incorrect. Please try again.");
            }

            //await _signInManager.SignInAsync(user, false, "Password");

            var generateModel = new GenerateTokensModel(user, request.UserAgent, request.UserIp);

            var (tokens, deviceId) = await _tokenService.IssueAsync(generateModel, cancellationToken);
            if (tokens is null || string.IsNullOrEmpty(tokens?.AccessToken) || string.IsNullOrEmpty(tokens?.RefreshToken))
            {
                _logger.LogCritical("Can't create access or refresh tokens for user with id '{id}'.", user.Id);
                throw new InvalidOperationException("Can't create access or refresh tokens.");
            }

            _logger.LogInformation("Success signin for user: {username}.", request.Username);

            return new AuthenticationDto
            {
                AccessToken = tokens.AccessToken,
                AccessTokenExpiredAt = tokens.AccessTokenExpiredAt,
                RefreshToken = tokens.RefreshToken,
                RefreshTokenExpiredAt = tokens.RefreshTokenExpiredAt,
                UserId = user.Id,
                DeviceId = deviceId
            };
        }
    }
}
