using Authentication.Application.Dtos;
using Authentication.Application.Models;
using Common.Blocks.Models.DomainResults;
using MediatR;

namespace Authentication.Application.Features.Authentications.Commands.RefreshToken
{
    public record RefreshTokenCommand : UserCredential, IRequest<Result<AuthenticationDto>>
    {
        public required Guid UserId { get; set; }
        public required string RefreshToken { get; set; }
        public required string DeviceId { get; set; }
    }
}
