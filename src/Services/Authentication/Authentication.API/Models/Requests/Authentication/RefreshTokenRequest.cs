namespace Authentication.API.Models.Requests.Authentication
{
    public class RefreshTokenRequest
    {
        public required Guid UserId { get; set; }
        public required string DeviceId { get; set; }
    }
}
