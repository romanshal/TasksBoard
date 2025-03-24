using Authentication.Application.Dtos;
using Authentication.Domain.Entities;
using Google.Apis.Auth;

namespace Authentication.Application.Interfaces.Services
{
    public interface ITokenService
    {
        Task<GoogleJsonWebSignature.Payload> VerifyGoogleToken(string? provider, string? tokenId);
        Task<AuthenticationDto> GenerateTokenAsync(ApplicationUser user);
        Task<AuthenticationDto> RefreshTokenAsync(ApplicationUser user);
    }
}
