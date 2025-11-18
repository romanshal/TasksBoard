using Authentication.Application.Dtos;
using Authentication.Application.Models;
using Common.Blocks.Models.DomainResults;
using MediatR;

namespace Authentication.Application.Features.Authentications.Commands.VerifyTwoFactor
{
    public sealed record VerifyTwoFactorCommand(
        Guid UserId,
        string Provider,
        string Code, 
        bool RememberMachine) : UserCredential, IRequest<Result<AuthenticationDto>>;
}
