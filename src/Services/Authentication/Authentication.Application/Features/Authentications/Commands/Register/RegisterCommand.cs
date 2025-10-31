using Authentication.Application.Dtos;
using Authentication.Application.Models;
using Common.Blocks.Models.DomainResults;
using MediatR;

namespace Authentication.Application.Features.Authentications.Commands.Register
{
    public record RegisterCommand : UserOption, IRequest<Result<AuthenticationDto>>
    {
        public required string Username { get; set; }
        public required string Email { get; set; }
        public required string Password { get; set; }
    }
}
