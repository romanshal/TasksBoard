using Authentication.Application.Dtos;
using Authentication.Domain.Entities;
using Authentication.Domain.Interfaces.Secutiry;
using Authentication.Domain.Models;
using Common.Blocks.Exceptions;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;

namespace Authentication.Application.Features.Authentications.Commands.RefreshToken
{
    public class RefreshTokenCommandHandler(
        UserManager<ApplicationUser> userManager,
        ITokenManager tokenService,
        ILogger<RefreshTokenCommandHandler> logger) : IRequestHandler<RefreshTokenCommand, AuthenticationDto>
    {
        private readonly UserManager<ApplicationUser> _userManager = userManager;
        private readonly ITokenManager _tokenService = tokenService;
        private readonly ILogger<RefreshTokenCommandHandler> _logger = logger;

        public async Task<AuthenticationDto> Handle(RefreshTokenCommand request, CancellationToken cancellationToken)
        {
            var user = await _userManager.FindByIdAsync(request.UserId.ToString());
            if (user is null)
            {
                _logger.LogWarning("User with name {userId} not found.", request.UserId);
                throw new UnauthorizedException($"User was not found.");
            }

            var generateModel = new GenerateTokensModel(user, request.UserAgent, request.UserIp);

            var (tokens, deviceId) = await _tokenService.RotateAsync(generateModel, request.RefreshToken, request.DeviceId, cancellationToken);

            if (tokens is null || string.IsNullOrEmpty(tokens?.AccessToken) || string.IsNullOrEmpty(tokens?.RefreshToken))
            {
                _logger.LogCritical("Can't create access or refresh tokens for user {id}.", user.Id);
                throw new UnauthorizedException("Can't create access or refresh tokens.");
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
