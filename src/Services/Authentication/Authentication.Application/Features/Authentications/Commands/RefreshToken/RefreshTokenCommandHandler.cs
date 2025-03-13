using Authentication.Application.Dtos;
using Authentication.Application.Models;
using Authentication.Application.Providers;
using Authentication.Domain.Entities;
using Common.Blocks.Exceptions;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.JsonWebTokens;

namespace Authentication.Application.Features.Authentications.Commands.RefreshToken
{
    public class RefreshTokenCommandHandler(
        UserManager<ApplicationUser> userManager,
        TokenProvider tokenProvider,
        ILogger<RefreshTokenCommandHandler> logger) : IRequestHandler<RefreshTokenCommand, AuthenticationDto>
    {
        private readonly UserManager<ApplicationUser> _userManager = userManager;
        private readonly TokenProvider _tokenProvider = tokenProvider;
        private readonly ILogger<RefreshTokenCommandHandler> _logger = logger;

        public async Task<AuthenticationDto> Handle(RefreshTokenCommand request, CancellationToken cancellationToken)
        {
            var user = await _userManager.FindByNameAsync(request.Username);
            if (user is null)
            {
                _logger.LogWarning($"User with name {request.Username} not found.");
                throw new UnauthorizedException($"User with name {request.Username} not found.");
            }

            var storedResfreshToken = await _userManager.GetAuthenticationTokenAsync(user, JwtConstants.TokenType, "refresh_token");
            if (string.IsNullOrEmpty(storedResfreshToken))
            {
                _logger.LogWarning($"No refresh tokens for user: {request.Username}.");
                throw new UnauthorizedException("Invalid refresh token.");
            }

            var userClaims = await _userManager.GetClaimsAsync(user);

            var token = _tokenProvider.Refresh(new RefreshTokenModel(user, userClaims, request.RefreshToken, storedResfreshToken));

            return token;
        }
    }
}
