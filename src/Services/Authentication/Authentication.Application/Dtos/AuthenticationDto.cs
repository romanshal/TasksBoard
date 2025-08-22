using Authentication.Domain.Models;

namespace Authentication.Application.Dtos
{
    public record AuthenticationDto : TokenPairModel
    {
        public Guid UserId { get; set; }
    }
}
