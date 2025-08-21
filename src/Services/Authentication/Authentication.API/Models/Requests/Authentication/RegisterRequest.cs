namespace Authentication.API.Models.Requests.Authentication
{
    public class RegisterRequest
    {
        public required string Username { get; set; }
        public required string Email { get; set; }
        public required string Password { get; set; }
        public required string DeviceId { get; set; }
    }
}
