using Authentication.Application.Models;
using Authentication.Domain.Entities;

namespace Authentication.Application.Interfaces.Providers
{
    public interface ITokenProvider
    {
        Task<TokenModel> GenerateAccessTokenAsync(ApplicationUser user);
        TokenModel GenerateRefreshToken();
    }
}
