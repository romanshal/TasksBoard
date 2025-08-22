using Authentication.Domain.Entities;
using Authentication.Infrastructure.Models;

namespace Authentication.Infrastructure.Security
{
    public interface ITokenFactory
    {
        Task<TokenModel> GenerateAccessTokenAsync(ApplicationUser user);
        TokenModel GenerateRefreshToken();
    }
}
