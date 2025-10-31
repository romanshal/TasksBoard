using Authentication.Application.Dtos;
using Authentication.Domain.Constants.AuthenticationErrors;
using Authentication.Domain.Entities;
using Authentication.Domain.Interfaces.Secutiry;
using Authentication.Domain.Models;
using Common.Blocks.Models.DomainResults;
using Microsoft.Extensions.Logging;

namespace Authentication.Application.Handlers
{
    /// <summary>
    /// Facad for generate token pair (access and refresh tokens).
    /// </summary>
    /// <param name="tokenManager">Token manager.</param>
    /// <param name="logger">Logger.</param>
    internal sealed class SignInHandler(
        ITokenManager tokenManager,
        ILogger<SignInHandler> logger)
    {
        private readonly ITokenManager _tokenManager = tokenManager;
        private readonly ILogger<SignInHandler> _logger = logger;

        /// <summary>
        /// Use for signin.
        /// </summary>
        /// <param name="user">Application user.</param>
        /// <param name="userAgent">User Agent.</param>
        /// <param name="userIp">User ip.</param>
        /// <param name="cancellationToken">Cancellation token.</param>
        /// <returns>Authentication credentials.</returns>
        public async Task<Result<AuthenticationDto>> HandleAsync(
            ApplicationUser user, 
            string userAgent, 
            string userIp, 
            CancellationToken cancellationToken = default)
        {
            var generateModel = new GenerateTokensModel(user, userAgent, userIp);

            var (tokens, deviceId) = await _tokenManager.IssueAsync(generateModel, cancellationToken);
            if (tokens is null || string.IsNullOrEmpty(tokens?.AccessToken) || string.IsNullOrEmpty(tokens?.RefreshToken))
            {
                _logger.LogCritical("Can't create access or refresh tokens for user with id '{id}'.", user.Id);
                return Result.Failure<AuthenticationDto>(AuthenticationErrors.SigninFaulted);
            }

            _logger.LogInformation("Success signin for user: {username}.", user.UserName);

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

        /// <summary>
        /// Use for refresh access token by refresh token.
        /// </summary>
        /// <param name="user">Application user.</param>
        /// <param name="userAgent">User Agent.</param>
        /// <param name="userIp">User ip.</param>
        /// <param name="refreshToken">Refresh token.</param>
        /// <param name="deviceId">Device id.</param>
        /// <param name="cancellationToken">Cancellation token.</param>
        /// <returns>Authentication credentials.</returns>
        public async Task<Result<AuthenticationDto>> HandleAsync(
            ApplicationUser user,
            string userAgent,
            string userIp,
            string refreshToken,
            string deviceId,
            CancellationToken cancellationToken = default)
        {
            var generateModel = new GenerateTokensModel(user, userAgent, userIp);

            var tokens = await _tokenManager.RotateAsync(generateModel, refreshToken, deviceId, cancellationToken);

            if (tokens is null || string.IsNullOrEmpty(tokens?.AccessToken) || string.IsNullOrEmpty(tokens?.RefreshToken))
            {
                _logger.LogCritical("Can't create access or refresh tokens for user {id}.", user.Id);
                return Result.Failure<AuthenticationDto>(AuthenticationErrors.SigninFaulted);
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
