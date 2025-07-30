using Authentication.Domain.Entities;
using System.Security.Claims;

namespace Authentication.Application.Models
{
    public class RefreshTokenModel(
        ApplicationUser user,
        IEnumerable<Claim> userClaims,
        string refreshToken,
        string storedRefreshToken) : CreateTokenModel(user, userClaims)
    {
        public string RefreshToken { get; set; } = refreshToken;
        public string StoredRefreshToken { get; set; } = storedRefreshToken;
    }
}
