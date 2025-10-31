using Authentication.Domain.Models;

namespace Authentication.Application.Dtos
{
    public record AuthenticationDto : TokenPairModel
    {
        public bool TwoFactorEnabled { get; set; } = false;
        public required Guid UserId { get; set; }
        public required string? DeviceId { get; set; }
    }
}
