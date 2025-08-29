namespace Authentication.Domain.Models
{
    public record TokenPairModel
    {
        public required string AccessToken { get; set; }
        public required DateTime AccessTokenExpiredAt { get; set; }
        public required string RefreshToken { get; set; }
        public required DateTime RefreshTokenExpiredAt { get; set; }
    }
}
