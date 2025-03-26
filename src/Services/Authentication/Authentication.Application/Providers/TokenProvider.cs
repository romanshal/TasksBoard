using Authentication.Application.Dtos;
using Authentication.Application.Interfaces.Providers;
using Authentication.Application.Models;
using Common.Blocks.Configurations;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.JsonWebTokens;
using Microsoft.IdentityModel.Tokens;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;

namespace Authentication.Application.Providers
{
    public class TokenProvider(IOptions<JwtCofiguration> options) : ITokenProvider
    {
        private readonly JwtCofiguration _jwtConfig = options.Value;

        public TokenDto Create(CreateTokenModel model)
        {
            string secretKey = _jwtConfig.Secret;
            var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secretKey));

            var credentionals = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(model.UserClaims),
                Expires = DateTime.Now.AddMinutes(_jwtConfig.ExpirationInMinutes),
                SigningCredentials = credentionals,
                Issuer = _jwtConfig.Issuer,
                Audience = _jwtConfig.Audience,
            };

            var handler = new JsonWebTokenHandler();

            return new AuthenticationDto
            {
                AccessToken = handler.CreateToken(tokenDescriptor),
                RefreshToken = GetRefreshToken()
            };
        }

        public TokenDto Refresh(RefreshTokenModel request)
        {
            if (request.RefreshToken != request.StoredRefreshToken)
            {
                throw new SecurityTokenException("Invalid token.");
            }

            return Create(new CreateTokenModel
            {
                UserId = request.UserId,
                UserEmail = request.UserEmail,
                UserClaims = request.UserClaims
            });
        }

        private static string GetRefreshToken()
        {
            var randomNumber = new byte[32];
            using var rng = RandomNumberGenerator.Create();

            rng.GetBytes(randomNumber);

            return Convert.ToBase64String(randomNumber);
        }
    }
}
