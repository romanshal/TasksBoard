namespace Authentication.Domain.Models
{
    public record TokenPairModel
    {
        public required string AccessToken { get; set; }
        public required string RefreshToken { get; set; }
    }
}
