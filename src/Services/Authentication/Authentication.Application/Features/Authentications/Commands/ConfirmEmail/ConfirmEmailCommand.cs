using Authentication.Application.Dtos;
using Authentication.Application.Models;
using Common.Blocks.Models.DomainResults;
using MediatR;

namespace Authentication.Application.Features.Authentications.Commands.ConfirmEmail
{
    public record ConfirmEmailCommand : UserOption, IRequest<Result<AuthenticationDto>>
    {
        public required Guid UserId { get; set; }
        public required string Token { get; set; }
    }
}
