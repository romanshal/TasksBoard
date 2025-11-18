using Authentication.Application.Dtos;
using Authentication.Application.Models;
using Common.Blocks.Models.DomainResults;
using MediatR;

namespace Authentication.Application.Features.Authentications.Commands.Login
{
    public record LoginCommand : UserCredential, IRequest<Result<AuthenticationDto>>
    {
        public required string UsernameOrEmail { get; set; }
        public required string Password { get; set; }
        public required bool RememberMe { get; set; }
    }
}
