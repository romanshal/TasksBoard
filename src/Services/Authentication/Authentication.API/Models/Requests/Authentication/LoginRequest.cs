namespace Authentication.API.Models.Requests.Authentication
{
    public class LoginRequest
    {
        public required string UsernameOrEmail { get; set; }
        public required string Password { get; set; }
        public required bool RememberMe { get; set; }
    }
}
