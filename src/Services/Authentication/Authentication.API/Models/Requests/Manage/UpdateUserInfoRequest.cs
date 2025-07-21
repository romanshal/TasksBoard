namespace Authentication.API.Models.Requests.Manage
{
    public record UpdateUserInfoRequest
    {
        public required string Username { get; set; }
        public required string Email { get; set; }
        public string? Firstname { get; set; }
        public string? Surname { get; set; }
    }
}
