using Authentication.Domain.Entities;

namespace Authentication.Application.Models
{
    public class GenerateTokensModel
    {
        public required ApplicationUser User { get; set; }
        public required string DeviceId { get; set; }
        public required string UserAgent { get; set; }
        public required string IpAddress { get; set; }
    }
}
