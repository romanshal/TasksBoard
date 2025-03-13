using Authentication.Domain.Entities;
using System.Security.Claims;

namespace Authentication.Application.Models
{
    public class CreateTokenModel
    {
        public Guid UserId { get; set; } = Guid.Empty;
        public string UserEmail { get; set; } = string.Empty;
        public IEnumerable<Claim> UserClaims { get; set; } = [];

        public CreateTokenModel() { }

        public CreateTokenModel(ApplicationUser user, IEnumerable<Claim> userClaims)
        {
            UserId = user.Id;
            UserEmail = user.Email!;
            UserClaims = userClaims;
        }
    }
}
