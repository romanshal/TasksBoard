using Microsoft.AspNetCore.Http;

namespace Authentication.Application.Models.Requests.Manage
{
    public class UpdateUserImageRequest
    {
        public required IFormFile Image { get; set; }
    }
}
