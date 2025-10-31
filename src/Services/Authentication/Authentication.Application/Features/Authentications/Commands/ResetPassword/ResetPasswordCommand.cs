using Common.Blocks.Models.DomainResults;
using MediatR;

namespace Authentication.Application.Features.Authentications.Commands.ResetPassword
{
    public record ResetPasswordCommand(Guid UserId, string Token, string NewPassword) : IRequest<Result>;
}
