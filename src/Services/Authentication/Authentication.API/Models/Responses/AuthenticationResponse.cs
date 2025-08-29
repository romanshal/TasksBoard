namespace Authentication.API.Models.Responses
{
    public class AuthenticationResponse
    {
        public required Guid UserId { get; set; }
        public required string AccessToken { get; set; }
        public required DateTime ExpiredAt { get; set; }
    }
}
