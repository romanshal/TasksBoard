namespace Authentication.Application.Models.Requests.Manage
{
    public class ChangeUserPasswordRequest
    {
        public required string CurrentPassword { get; set; }
        public required string NewPassword { get; set; }
    }
}
