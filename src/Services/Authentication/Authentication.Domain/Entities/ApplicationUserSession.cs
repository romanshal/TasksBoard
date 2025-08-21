namespace Authentication.Domain.Entities
{
    public class ApplicationUserSession
    {
        public Guid Id { get; set; }
        public Guid UserId { get; set; }
        public string RefreshTokenHash { get; set; } = default!;
        public string RefreshTokenSalt { get; set; } = default!;
        public string IpAddress { get; set; } = default!;
        public string DeviceId { get; set; } = default!;
        public string UserAgent { get; set; } = default!;
        public DateTime ExpiresAtUtc { get; set; }
        public DateTime CreatedAtUtc { get; set; }
        public DateTime? RevokedAtUtc { get; set; }
        public Guid? ReplacedBySessionId { get; set; }

        public bool IsExpired => DateTime.UtcNow >= ExpiresAtUtc;
        public bool IsRevoked => RevokedAtUtc != null;
        public bool IsActive => !IsRevoked && !IsExpired;

        public virtual ApplicationUser User { get; set; } = default!;
        public virtual ApplicationUserSession? ReplacedBySession { get; set; }
        public virtual ApplicationUserSession? ReplaceSession { get; set; }
    }
}
