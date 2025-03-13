using Authentication.Domain.Entities;
using System.Security.Claims;

namespace Authentication.Application.Interfaces.Services
{
    public interface IUserClaimsService
    {
        Task<IEnumerable<Claim>> GetUserClaimsAsync(ApplicationUser user);
    }
}
