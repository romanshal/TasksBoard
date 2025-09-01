using Authentication.Domain.Entities;
using Authentication.Domain.Interfaces.Factories;
using Authentication.Domain.Interfaces.Repositories;
using Authentication.Domain.Interfaces.Secutiry;
using Authentication.Domain.Models;
using Authentication.Infrastructure.Models;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Tokens;

namespace Authentication.Infrastructure.Security
{
    public class TokenManager(
        ITokenFactory tokenFactory,
        ILogger<TokenManager> logger,
        IApplicationUserSessionRepository sessionRepository,
        IDeviceFactory deviceFactory) : ITokenManager
    {
        private readonly ITokenFactory _tokenFactory = tokenFactory;
        private readonly ILogger<TokenManager> _logger = logger;
        private readonly IApplicationUserSessionRepository _sessionRepository = sessionRepository;
        private readonly IDeviceFactory _deviceFactory = deviceFactory;

        public async Task<(TokenPairModel tokens, string deviceId)> IssueAsync(
            GenerateTokensModel model,
            CancellationToken cancellationToken = default)
        {
            var (accessToken, session) = await GenerateTokensAsync(model, cancellationToken: cancellationToken);
            var tokenPair = new TokenPairModel
            {
                AccessToken = accessToken.Token,
                AccessTokenExpiredAt = accessToken.ExpiresAtUtc,
                RefreshToken = session.Token,
                RefreshTokenExpiredAt = session.ExpiresAtUtc
            };

            return (tokenPair, session.DeviceId);
        }

        public async Task<(TokenPairModel tokens, string deviceId)> RotateAsync(
            GenerateTokensModel model,
            string oldRefreshToken,
            string deviceId,
            CancellationToken cancellationToken = default)
        {
            var token = await _sessionRepository.GetByUserIdAndDeviceId(model.User.Id, deviceId, cancellationToken);
            if (token is null || !token.IsActive)
            {
                throw new SecurityTokenException("Invalid token.");
            }

            var valid = TokenHasher.Verify(oldRefreshToken, token.RefreshTokenHash, token.RefreshTokenSalt);
            if (!valid)
            {
                throw new SecurityTokenException("Invalid token.");
            }

            var (newAccessToken, newSession) = await GenerateTokensAsync(model, deviceId, cancellationToken);

            token.RevokedAtUtc = DateTime.UtcNow;
            token.ReplacedBySessionId = newSession.SessionId;

            _sessionRepository.Update(token);

            var affectedRows = await _sessionRepository.SaveChangesAsync(cancellationToken);
            if (affectedRows == 0)
            {
                throw new Exception();
            }

            var tokenPair = new TokenPairModel
            {
                AccessToken = newAccessToken.Token,
                AccessTokenExpiredAt = newAccessToken.ExpiresAtUtc,
                RefreshToken = newSession.Token,
                RefreshTokenExpiredAt = newSession.ExpiresAtUtc
            };

            return (tokenPair, newSession.DeviceId);
        }

        public async Task RevokeAsync(
            ApplicationUser user,
            CancellationToken cancellationToken = default)
        {
            var sessions = await _sessionRepository.GetByUserIdAsync(user.Id, cancellationToken);

            foreach (var session in sessions)
            {
                if (session is null || !session.IsRevoked) continue;

                session.RevokedAtUtc = DateTime.UtcNow;
                _sessionRepository.Update(session);
            }

            await _sessionRepository.SaveChangesAsync(cancellationToken);
        }

        private async Task<(TokenModel accessToken, SessionModel session)> GenerateTokensAsync(
            GenerateTokensModel model,
            string? deviceId = null,
            CancellationToken cancellationToken = default)
        {
            var accessToken = await _tokenFactory.GenerateAccessTokenAsync(model.User);
            var refreshToken = _tokenFactory.GenerateRefreshToken();

            var (hash, salt) = TokenHasher.Hash(refreshToken.Token);

            deviceId = deviceId ?? _deviceFactory.GenerateId();

            var session = new ApplicationUserSession
            {
                UserId = model.User.Id,
                RefreshTokenHash = hash,
                RefreshTokenSalt = salt,
                IpAddress = model.IpAddress,
                DeviceId = deviceId!,
                UserAgent = model.UserAgent,
                CreatedAtUtc = DateTime.UtcNow,
                ExpiresAtUtc = refreshToken.ExpiresAtUtc
            };

            _sessionRepository.Add(session);
            await _sessionRepository.SaveChangesAsync(cancellationToken);

            var sessionModel = new SessionModel
            {
                SessionId = session.Id,
                DeviceId = session.DeviceId,
                Token = refreshToken.Token,
                ExpiresAtUtc = refreshToken.ExpiresAtUtc
            };

            return (accessToken, sessionModel);
        }
    }
}
