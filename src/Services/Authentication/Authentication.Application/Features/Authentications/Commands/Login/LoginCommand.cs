using Authentication.Application.Dtos;
using MediatR;

namespace Authentication.Application.Features.Authentications.Commands.Login
{
    public record LoginCommand : IRequest<AuthenticationDto>
    {
        public required string Username { get; set; }
        public required string Password { get; set; }
    }
}
