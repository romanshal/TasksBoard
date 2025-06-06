using Authentication.Application.Dtos;
using Authentication.Domain.Entities;
using Google.Apis.Auth;
using Microsoft.AspNetCore.Identity;

namespace Authentication.Application.Interfaces.Services
{
    public interface ITokenService
    {
        Task<GoogleJsonWebSignature.Payload> VerifyGoogleToken(string? provider, string? tokenId);
        Task<TokenDto> GenerateTokenAsync(ApplicationUser user);
        Task<TokenDto> RefreshTokenAsync(ApplicationUser user);
        Task<IdentityResult> DeleteRefreshToken(ApplicationUser user);
    }
}
