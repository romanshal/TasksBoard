using Authentication.Application.Dtos;
using Common.Blocks.Models.DomainResults;
using MediatR;

namespace Authentication.Application.Features.Authentications.Commands.GenerateAuthenticatorSetup
{
    public record GenerateAuthenticatorSetupCommand(Guid UserId, string Issuer) : IRequest<Result<AuthenticatorSetupDto>>;
}
