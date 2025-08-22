using Authentication.Domain.Entities;

namespace Authentication.Domain.Models
{
    public sealed record GenerateTokensModel
    {
        public ApplicationUser User { get; init; }
        public string DeviceId { get; init; }
        public string UserAgent { get; init; }
        public string IpAddress { get; init; }

        public GenerateTokensModel(
            ApplicationUser user,
            string deviceId,
            string userAgent,
            string ipAddress)
        {
            User = user ?? throw new ArgumentNullException(nameof(user));
            DeviceId = !string.IsNullOrWhiteSpace(deviceId) ? deviceId : throw new ArgumentException("DeviceId is required", nameof(deviceId));
            UserAgent = !string.IsNullOrWhiteSpace(userAgent) ? userAgent : throw new ArgumentException("UserAgent is required", nameof(userAgent));
            IpAddress = !string.IsNullOrWhiteSpace(ipAddress) ? ipAddress : throw new ArgumentException("IpAddress is required", nameof(ipAddress));
        }
    }
}
