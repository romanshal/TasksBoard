using Authentication.Application.Dtos;
using Authentication.Domain.Entities;

namespace Authentication.Application.Interfaces.Services
{
    public interface ITokenService
    {
        Task<AuthenticationDto> GenerateTokenAsync(ApplicationUser user);
        Task<AuthenticationDto> RefreshTokenAsync(ApplicationUser user);
    }
}
