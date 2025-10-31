using Authentication.Domain.Entities;
using Authentication.Domain.Models;

namespace Authentication.Domain.Interfaces.Secutiry
{
    public interface ITokenManager
    {
        Task<(TokenPairModel tokens, string deviceId)> IssueAsync(
            GenerateTokensModel model,
            CancellationToken cancellationToken = default);

        Task<TokenPairModel> RotateAsync(
            GenerateTokensModel model,
            string oldRefreshToken,
            string deviceId,
            CancellationToken cancellationToken = default);

        Task RevokeAsync(
            ApplicationUser user,
            CancellationToken cancellationToken = default);
    }
}
