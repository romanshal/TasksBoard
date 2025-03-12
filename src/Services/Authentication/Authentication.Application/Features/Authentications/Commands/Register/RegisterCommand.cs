using Authentication.Application.Dtos;
using MediatR;

namespace Authentication.Application.Features.Authentications.Commands.Register
{
    public class RegisterCommand : IRequest<AuthenticationDto>
    {
        public required string Username { get; set; }
        public required string Email { get; set; }
        public required string Password { get; set; }
    }
}
