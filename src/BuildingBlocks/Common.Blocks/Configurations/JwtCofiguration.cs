namespace Common.Blocks.Configurations
{
    public class JwtCofiguration
    {
        public required string Secret { get; set; }
        public required string Issuer { get; set; }
        public required string Audience { get; set; }
        public int ExpirationInMinutes { get; set; }
    }
}
