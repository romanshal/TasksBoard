using Authentication.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

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
