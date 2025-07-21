namespace Authentication.API.Models.Requests.Manage
{
    public record ChangeUserPasswordRequest
    {
        public required string CurrentPassword { get; set; }
        public required string NewPassword { get; set; }
    }
}
