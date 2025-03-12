namespace Authentication.Application.Models
{
    public class RefreshTokenModel : CreateTokenModel
    {
        public required string RefreshToken { get; set; }
        public required string StoredRefreshToken { get; set; }
    }
}
