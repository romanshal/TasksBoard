using Authentication.Application.Configurations;
using Authentication.Application.Dtos;
using Authentication.Application.Models;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.JsonWebTokens;
using Microsoft.IdentityModel.Tokens;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;

namespace Authentication.Application.Providers
{
    public class TokenProvider(IOptions<JwtCofiguration> options)
    {
        private readonly JwtCofiguration _jwtConfig = options.Value;

        public AuthenticationDto Create(CreateTokenModel model)
        {
            string secretKey = _jwtConfig.Secret;
            var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secretKey));

            var credentionals = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(
                [
                    new Claim(JwtRegisteredClaimNames.Sub, model.UserId.ToString()),
                    new Claim(JwtRegisteredClaimNames.Email, model.UserEmail!)
                ]),
                Expires = DateTime.Now.AddMinutes(_jwtConfig.ExpirationInMinutes),
                SigningCredentials = credentionals,
                Issuer = _jwtConfig.Issuer,
                Audience = _jwtConfig.Audience
            };

            var handler = new JsonWebTokenHandler();

            return new AuthenticationDto
            {
                AccessToken = handler.CreateToken(tokenDescriptor),
                RefreshToken = GetRefreshToken()
            };
        }

        public AuthenticationDto Refresh(RefreshTokenModel request)
        {
            if (request.RefreshToken != request.StoredRefreshToken)
            {
                throw new SecurityTokenException("Invalid token.");
            }

            return Create(new CreateTokenModel
            {
                UserId = request.UserId,
                UserEmail = request.UserEmail,
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
