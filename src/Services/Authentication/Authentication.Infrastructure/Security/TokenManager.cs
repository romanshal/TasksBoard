using Authentication.Domain.Entities;
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
        IApplicationUserSessionRepository sessionRepository) : ITokenManager
    {
        private readonly ITokenFactory _tokenFactory = tokenFactory;
        private readonly ILogger<TokenManager> _logger = logger;
        private readonly IApplicationUserSessionRepository _sessionRepository = sessionRepository;

        public async Task<TokenPairModel> IssueAsync(
            GenerateTokensModel model,
            CancellationToken cancellationToken = default)
        {
            var (accessToken, refreshToken) = await GenerateTokensAsync(model, cancellationToken);

            return new TokenPairModel
            {
                AccessToken = accessToken.Token,
                AccessTokenExpiredAt = accessToken.ExpiresAtUtc,
                RefreshToken = refreshToken.Token,
                RefreshTokenExpiredAt = refreshToken.ExpiresAtUtc
            };
        }

        public async Task<TokenPairModel> RotateAsync(
            GenerateTokensModel model,
            string oldRefreshToken,
            CancellationToken cancellationToken = default)
        {
            var token = await _sessionRepository.GetByUserIdAndDeviceId(model.User.Id, model.DeviceId, cancellationToken);
            if (token is null || !token.IsActive)
            {
                throw new SecurityTokenException("Invalid token.");
            }

            var valid = TokenHasher.Verify(oldRefreshToken, token.RefreshTokenHash, token.RefreshTokenSalt);
            if (!valid)
            {
                throw new SecurityTokenException("Invalid token.");
            }

            var (newAccessToken, newRefreshToken) = await GenerateTokensAsync(model, cancellationToken);

            token.RevokedAtUtc = DateTime.UtcNow;
            token.ReplacedBySessionId = newRefreshToken.SessionId;

            _sessionRepository.Update(token);

            var affectedRows = await _sessionRepository.SaveChangesAsync(cancellationToken);
            if (affectedRows == 0)
            {
                throw new Exception();
            }

            return new TokenPairModel
            {
                AccessToken = newAccessToken.Token,
                AccessTokenExpiredAt = newAccessToken.ExpiresAtUtc,
                RefreshToken = newRefreshToken.Token,
                RefreshTokenExpiredAt = newRefreshToken.ExpiresAtUtc
            };
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

        private async Task<(TokenModel accessToken, SessionModel refreshToken)> GenerateTokensAsync(
            GenerateTokensModel model,
            CancellationToken cancellationToken = default)
        {
            var accessToken = await _tokenFactory.GenerateAccessTokenAsync(model.User);
            var refreshToken = _tokenFactory.GenerateRefreshToken();

            var (hash, salt) = TokenHasher.Hash(refreshToken.Token);

            var session = new ApplicationUserSession
            {
                UserId = model.User.Id,
                RefreshTokenHash = hash,
                RefreshTokenSalt = salt,
                IpAddress = model.IpAddress,
                DeviceId = model.DeviceId,
                UserAgent = model.UserAgent,
                CreatedAtUtc = DateTime.UtcNow,
                ExpiresAtUtc = refreshToken.ExpiresAtUtc
            };

            _sessionRepository.Add(session);
            await _sessionRepository.SaveChangesAsync(cancellationToken);

            var sessionModel = new SessionModel
            {
                SessionId = session.Id,
                Token = refreshToken.Token,
                ExpiresAtUtc = refreshToken.ExpiresAtUtc
            };

            return (accessToken, sessionModel);
        }
    }
}
