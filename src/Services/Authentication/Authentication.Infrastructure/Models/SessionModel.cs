namespace Authentication.Infrastructure.Models
{
    public class SessionModel : TokenModel
    {
        public required Guid SessionId { get; set; } 
        public required string DeviceId { get; set; }
    }
}
