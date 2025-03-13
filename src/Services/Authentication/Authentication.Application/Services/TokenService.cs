using Authentication.Application.Dtos;
using Authentication.Application.Interfaces.Providers;
using Authentication.Application.Interfaces.Services;
using Authentication.Application.Models;
using Authentication.Domain.Entities;
using Common.Blocks.Exceptions;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using System.IdentityModel.Tokens.Jwt;

namespace Authentication.Application.Services
{
    public class TokenService(
        ITokenProvider tokenProvider,
        IUserClaimsService userClaims,
        UserManager<ApplicationUser> userManager,
        ILogger<TokenService> logger) : ITokenService
    {
        private readonly ITokenProvider _tokenProvider = tokenProvider;
        private readonly IUserClaimsService _userClaims = userClaims;
        private readonly UserManager<ApplicationUser> _userManager = userManager;
        private readonly ILogger<TokenService> _logger = logger;

        public async Task<AuthenticationDto> GenerateTokenAsync(ApplicationUser user)
        {
            var claims = await _userClaims.GetUserClaimsAsync(user);

            var token = _tokenProvider.Create(new CreateTokenModel(user, claims));

            var saveTokenResult = await _userManager.SetAuthenticationTokenAsync(user, JwtConstants.TokenType, "refresh_token", token.RefreshToken);
            if (!saveTokenResult.Succeeded)
            {
                _logger.LogCritical($"Can't save refresh token for user: {user.UserName}. Errors: {string.Join("; ", saveTokenResult.Errors)}");
                throw new Exception($"Can't save refresh token for user: {user.UserName}. Errors: {string.Join("; ", saveTokenResult.Errors)}");
            }

            return token;
        }

        public async Task<AuthenticationDto> RefreshTokenAsync(ApplicationUser user)
        {
            var storedResfreshToken = await _userManager.GetAuthenticationTokenAsync(user, JwtConstants.TokenType, "refresh_token");
            if (string.IsNullOrEmpty(storedResfreshToken))
            {
                _logger.LogWarning($"No refresh tokens for user: {user.UserName}.");
                throw new UnauthorizedException("Invalid refresh token.");
            }

            return await GenerateTokenAsync(user);
        }
    }
}
