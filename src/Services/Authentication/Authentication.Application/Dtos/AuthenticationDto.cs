namespace Authentication.Application.Dtos
{
    public class AuthenticationDto
    {
        public required string AccessToken { get; set; }
        public required string RefreshToken { get; set; }
    }
}
