using Authentication.Domain.Entities;

namespace Authentication.Domain.Models
{
    public sealed record GenerateTokensModel
    {
        public ApplicationUser User { get; init; }
        public string UserAgent { get; init; }
        public string IpAddress { get; init; }

        public GenerateTokensModel(
            ApplicationUser user,
            string userAgent,
            string ipAddress)
        {
            User = user ?? throw new ArgumentNullException(nameof(user));
            UserAgent = !string.IsNullOrWhiteSpace(userAgent) ? userAgent : throw new ArgumentException("UserAgent is required", nameof(userAgent));
            IpAddress = !string.IsNullOrWhiteSpace(ipAddress) ? ipAddress : throw new ArgumentException("IpAddress is required", nameof(ipAddress));
        }
    }
}
