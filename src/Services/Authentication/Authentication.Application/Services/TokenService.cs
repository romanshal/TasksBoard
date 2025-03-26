using Authentication.Application.Dtos;
using Authentication.Application.Interfaces.Providers;
using Authentication.Application.Interfaces.Services;
using Authentication.Application.Models;
using Authentication.Domain.Entities;
using Common.Blocks.Exceptions;
using Google.Apis.Auth;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System.IdentityModel.Tokens.Jwt;

namespace Authentication.Application.Services
{
    public class TokenService(
        ITokenProvider tokenProvider,
        IUserClaimsService userClaims,
        UserManager<ApplicationUser> userManager,
        IConfiguration configuration,
        ILogger<TokenService> logger) : ITokenService
    {
        private readonly ITokenProvider _tokenProvider = tokenProvider;
        private readonly IUserClaimsService _userClaims = userClaims;
        private readonly UserManager<ApplicationUser> _userManager = userManager;
        private readonly ILogger<TokenService> _logger = logger;
        private readonly IConfiguration _configuration = configuration;

        public async Task<TokenDto> GenerateTokenAsync(ApplicationUser user)
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

        public async Task<TokenDto> RefreshTokenAsync(ApplicationUser user)
        {
            var storedResfreshToken = await _userManager.GetAuthenticationTokenAsync(user, JwtConstants.TokenType, "refresh_token");
            if (string.IsNullOrEmpty(storedResfreshToken))
            {
                _logger.LogWarning($"No refresh tokens for user: {user.UserName}.");
                throw new UnauthorizedException("Invalid refresh token.");
            }

            return await GenerateTokenAsync(user);
        }

        public async Task<GoogleJsonWebSignature.Payload> VerifyGoogleToken(string? provider, string? tokenId)
        {
            var clientId = _configuration["Authentication:Google:ClientId"] ?? throw new InvalidOperationException("Configuration setting 'Google:ClientId' not found.");

            var settings = new GoogleJsonWebSignature.ValidationSettings()
            {
                Audience = [clientId]
            };

            var payload = await GoogleJsonWebSignature.ValidateAsync(tokenId, settings);

            return payload;
        }
    }
}
