using Authentication.Application.Dtos;
using Authentication.Application.Models;
using MediatR;

namespace Authentication.Application.Features.Authentications.Commands.RefreshToken
{
    public record RefreshTokenCommand : UserOption, IRequest<AuthenticationDto>
    {
        public required Guid UserId { get; set; }
        public required string RefreshToken { get; set; }
        public required string DeviceId { get; set; }
    }
}
