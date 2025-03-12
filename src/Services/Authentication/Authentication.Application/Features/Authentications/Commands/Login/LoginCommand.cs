using Authentication.Application.Dtos;
using MediatR;

namespace Authentication.Application.Features.Authentications.Commands.Login
{
    public class LoginCommand : IRequest<AuthenticationDto>
    {
        public required string Username { get; set; }
        public required string Password { get; set; }
    }
}
