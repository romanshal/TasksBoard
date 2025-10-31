using Common.Blocks.Models.DomainResults;
using MediatR;

namespace Authentication.Application.Features.Authentications.Commands.GenerateTwoFactorCode
{
    public record GenerateTwoFactorCodeCommand(Guid UserId, string Provider) : IRequest<Result>;
}
