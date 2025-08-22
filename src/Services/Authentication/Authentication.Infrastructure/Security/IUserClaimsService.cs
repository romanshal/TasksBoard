using Authentication.Domain.Entities;
using System.Security.Claims;

namespace Authentication.Infrastructure.Security
{
    public interface IUserClaimsService
    {
        Task<IEnumerable<Claim>> GetUserClaimsAsync(ApplicationUser user);
    }
}
