using Authentication.Application.Dtos;
using Authentication.Application.Models;
using Common.Blocks.Models.DomainResults;
using MediatR;

namespace Authentication.Application.Features.Authentications.Commands.ExternalLoginCallback
{
    public record ExternalLoginCallbackCommand : UserOption, IRequest<Result<AuthenticationDto>>
    {
    }
}
