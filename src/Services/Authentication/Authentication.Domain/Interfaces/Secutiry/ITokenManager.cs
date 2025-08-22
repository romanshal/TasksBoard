using Authentication.Domain.Entities;
using Authentication.Domain.Models;

namespace Authentication.Domain.Interfaces.Secutiry
{
    public interface ITokenManager
    {
        Task<TokenPairModel> IssueAsync(
            GenerateTokensModel model,
            CancellationToken cancellationToken = default);

        Task<TokenPairModel> RotateAsync(
            GenerateTokensModel model,
            string oldRefreshToken,
            CancellationToken cancellationToken = default);

        Task RevokeAsync(
            ApplicationUser user,
            CancellationToken cancellationToken = default);
    }
}
