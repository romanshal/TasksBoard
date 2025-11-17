using Common.Blocks.Models.DomainResults;
using MediatR;

namespace Authentication.Application.Features.Authentications.Commands.GenerateEmailConfirmToken
{
    public sealed record GenerateEmailConfirmTokenCommand : IRequest<Result>
    {
        public required Guid UserId { get; set; }
    }
}
