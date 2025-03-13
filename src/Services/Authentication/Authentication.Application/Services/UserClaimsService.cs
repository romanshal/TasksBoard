using Authentication.Application.Interfaces.Services;
using Authentication.Domain.Entities;
using Microsoft.AspNetCore.Identity;
using System.Security.Claims;

namespace Authentication.Application.Services
{
    public class UserClaimsService(
        UserManager<ApplicationUser> userManager,
        RoleManager<ApplicationRole> roleManager) : IUserClaimsService
    {
        private readonly UserManager<ApplicationUser> _userManager = userManager;
        private readonly RoleManager<ApplicationRole> _roleManager = roleManager;

        public async Task<IEnumerable<Claim>> GetUserClaimsAsync(ApplicationUser user)
        {
            var claims = new List<Claim>();

            //user roles claims
            var userRoles = await _userManager.GetRolesAsync(user);
            foreach (var userRole in userRoles)
            {
                claims.Add(new Claim(ClaimTypes.Role, userRole));
            }

            //user clams
            var userClaims = await _userManager.GetClaimsAsync(user);
            claims.AddRange(userClaims);

            //role claims
            foreach (var userRole in userRoles)
            {
                var role = await _roleManager.FindByNameAsync(userRole);
                var roleClaims = await _roleManager.GetClaimsAsync(role!);

                claims.AddRange(roleClaims);
            }

            return claims;
        }
    }
}
