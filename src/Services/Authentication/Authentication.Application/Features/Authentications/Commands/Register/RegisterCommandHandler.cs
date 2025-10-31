using Authentication.Application.Dtos;
using Authentication.Domain.Constants.AuthenticationErrors;
using Authentication.Domain.Entities;
using Authentication.Domain.Interfaces.Secutiry;
using Authentication.Domain.Models;
using Common.Blocks.Exceptions;
using Common.Blocks.Models.DomainResults;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;

namespace Authentication.Application.Features.Authentications.Commands.Register
{
    public class RegisterCommandHandler(
        UserManager<ApplicationUser> userManager,
        SignInManager<ApplicationUser> signInManager,
        ITokenManager tokenService,
        ILogger<RegisterCommandHandler> logger) : IRequestHandler<RegisterCommand, Result<AuthenticationDto>>
    {
        private readonly UserManager<ApplicationUser> _userManager = userManager;
        private readonly SignInManager<ApplicationUser> _signInManager = signInManager;
        private readonly ITokenManager _tokenService = tokenService;
        private readonly ILogger<RegisterCommandHandler> _logger = logger;

        public async Task<Result<AuthenticationDto>> Handle(RegisterCommand request, CancellationToken cancellationToken)
        {
            var user = await _userManager.FindByNameAsync(request.Username);
            if (user is not null)
            {
                return Result.Failure<AuthenticationDto>(AuthenticationErrors.AlreadyExist(request.Username));
            }

            user = new ApplicationUser
            {
                Email = request.Email,
                UserName = request.Username
            };

            var createResult = await _userManager.CreateAsync(user, request.Password);
            if (!createResult.Succeeded)
            {
                _logger.LogCritical("Can't create new user with username: {username}. Errors: {errors}.", request.Username, string.Join("; ", createResult.Errors));
                return Result.Failure<AuthenticationDto>(AuthenticationErrors.SignupFaulted);
            }

            var addRoleResult = await _userManager.AddToRoleAsync(user, "user");

            if (!addRoleResult.Succeeded)
            {
                _logger.LogCritical("Can't add role to user: {username}. Errors: {errors}.", request.Username, string.Join("; ", addRoleResult.Errors));
                return Result.Failure<AuthenticationDto>(AuthenticationErrors.SignupFaulted);
            }

            await _signInManager.SignInAsync(user, false, "Password");

            var generateModel = new GenerateTokensModel(user, request.UserAgent, request.UserIp);

            var (tokens, deviceId) = await _tokenService.IssueAsync(generateModel, cancellationToken);

            _logger.LogInformation("Success register for user: {username}.", request.Username);
            if (tokens is null || string.IsNullOrEmpty(tokens?.AccessToken) || string.IsNullOrEmpty(tokens?.RefreshToken))
            {
                _logger.LogCritical("Can't create access or refresh tokens for user {id}.", user.Id);
                return Result.Failure<AuthenticationDto>(AuthenticationErrors.SignupFaulted);
            }

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
