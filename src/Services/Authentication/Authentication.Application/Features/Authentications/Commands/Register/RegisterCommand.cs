using Authentication.Application.Dtos;
using Authentication.Application.Models;
using MediatR;

namespace Authentication.Application.Features.Authentications.Commands.Register
{
    public record RegisterCommand : UserOption, IRequest<AuthenticationDto>
    {
        public required string Username { get; set; }
        public required string Email { get; set; }
        public required string Password { get; set; }
    }
}
