using Common.Blocks.Models.DomainResults;
using MediatR;

namespace Authentication.Application.Features.Authentications.Commands.ForgotPassword
{
    public record ForgotPasswordCommand(string Email) : IRequest<Result>;
}
