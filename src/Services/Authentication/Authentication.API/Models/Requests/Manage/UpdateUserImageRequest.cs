namespace Authentication.API.Models.Requests.Manage
{
    public record UpdateUserImageRequest
    {
        public required IFormFile Image { get; set; }
    }
}
