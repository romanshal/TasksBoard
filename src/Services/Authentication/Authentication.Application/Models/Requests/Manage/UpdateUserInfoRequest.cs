namespace Authentication.Application.Models.Requests.Manage
{
    public class UpdateUserInfoRequest
    {
        public required string Username { get; set; }
        public required string Email { get; set; }
        public string? Firstname { get; set; }
        public string? Surname { get; set; }
    }
}
