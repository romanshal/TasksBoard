using Authentication.Domain.Entities;
using Authentication.Infrastructure.Models;
using Common.Blocks.Configurations;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.JsonWebTokens;
using Microsoft.IdentityModel.Tokens;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;

namespace Authentication.Infrastructure.Security.JWT
{
    public class TokenFactory(
        IOptions<JwtCofiguration> options,
        IUserClaimsService userClaims) : ITokenFactory
    {
        private readonly JwtCofiguration _jwtConfig = options.Value;
        private readonly IUserClaimsService _userClaims = userClaims;

        public async Task<TokenModel> GenerateAccessTokenAsync(ApplicationUser user)
        {
            string secretKey = _jwtConfig.Secret;

            var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secretKey));

            var credentionals = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

            var now = DateTime.Now;

            var claims = await _userClaims.GetUserClaimsAsync(user);

            var exp = now.AddMinutes(_jwtConfig.AccessTokenExpirationMinutes);

            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(claims),
                Expires = exp,
                SigningCredentials = credentionals,
                Issuer = _jwtConfig.Issuer,
                Audience = _jwtConfig.Audience,
                NotBefore = now
            };

            var handler = new JsonWebTokenHandler();
            var accessToken = handler.CreateToken(tokenDescriptor);

            return new TokenModel
            {
                Token = accessToken,
                ExpiresAtUtc = exp
            };
        }

        public TokenModel GenerateRefreshToken()
        {
            var randomNumber = new byte[64];
            using var rng = RandomNumberGenerator.Create();

            rng.GetBytes(randomNumber);

            var now = DateTime.UtcNow;
            var exp = now.AddDays(_jwtConfig.RefreshTokenExpirationDays);

            return new TokenModel
            {
                Token = Convert.ToBase64String(randomNumber),
                ExpiresAtUtc = exp
            };
        }
    }
}
