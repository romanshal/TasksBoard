using Authentication.Domain.Entities;
using Microsoft.AspNetCore.Identity;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

namespace Authentication.Infrastructure.Security.JWT
{
    public class UserClaimsService(
        UserManager<ApplicationUser> userManager,
        RoleManager<ApplicationRole> roleManager) : IUserClaimsService
    {
        private readonly UserManager<ApplicationUser> _userManager = userManager;
        private readonly RoleManager<ApplicationRole> _roleManager = roleManager;

        public async Task<IEnumerable<Claim>> GetUserClaimsAsync(ApplicationUser user)
        {
            var claims = new List<Claim>
            {
                new(ClaimTypes.Name, user.UserName!),
                new(ClaimTypes.Email, user.Email!),
                new(ClaimTypes.NameIdentifier, user.Id.ToString()),
                new(JwtRegisteredClaimNames.Sub, user.Id.ToString()),
                new(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
            };

            //user roles claims
            var userRoles = await _userManager.GetRolesAsync(user);
            foreach (var userRole in userRoles)
            {
                claims.Add(new Claim(ClaimTypes.Role, userRole));
            }

            //user clams
            var userClaims = await _userManager.GetClaimsAsync(user);
            claims.AddRange(userClaims);

            //user stamp
            var userStamp = await _userManager.GetSecurityStampAsync(user);
            claims.Add(new Claim("sst", userStamp));

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
