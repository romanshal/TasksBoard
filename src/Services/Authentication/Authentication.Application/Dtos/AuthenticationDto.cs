using Authentication.Domain.Models;

namespace Authentication.Application.Dtos
{
    public record AuthenticationDto : TokenPairModel
    {
        public required Guid UserId { get; set; }
        public required string DeviceId { get; set; }
    }
}
