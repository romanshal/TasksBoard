using Authentication.Application.Dtos;
using Authentication.Application.Models;
using MediatR;

namespace Authentication.Application.Features.Authentications.Commands.Login
{
    public record LoginCommand : UserOption, IRequest<AuthenticationDto>
    {
        public required string Username { get; set; }
        public required string Password { get; set; }
    }
}
