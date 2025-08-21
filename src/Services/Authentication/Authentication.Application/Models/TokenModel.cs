namespace Authentication.Application.Models
{
    public class TokenModel
    {
        public required string Token { get; set; }
        public required DateTime ExpiresAtUtc { get; set; }
    }
}
