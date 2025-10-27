using Microsoft.AspNetCore.Identity;

namespace Authentication.Domain.Entities
{
    public class ApplicationUser : IdentityUser<Guid>
    {
        public string? FirstName { get; set; }
        public string? Surname { get; set; }

        public virtual ApplicationUserImage Image { get; set; } = default!;
        public virtual ICollection<ApplicationUserSession> RefreshTokens { get; set; } = [];
    }
}
