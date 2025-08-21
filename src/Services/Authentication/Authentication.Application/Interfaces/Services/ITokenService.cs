using Authentication.Application.Dtos;
using Authentication.Application.Models;
using Authentication.Domain.Entities;

namespace Authentication.Application.Interfaces.Services
{
    public interface ITokenService
    {
        Task<TokenPairDto> IssueAsync(
            GenerateTokensModel model,
            CancellationToken cancellationToken = default);

        Task<TokenPairDto> RotateAsync(
            GenerateTokensModel model,
            string oldRefreshToken,
            CancellationToken cancellationToken = default);

        Task RevokeAsync(
            ApplicationUser user,
            CancellationToken cancellationToken = default);
    }
}
