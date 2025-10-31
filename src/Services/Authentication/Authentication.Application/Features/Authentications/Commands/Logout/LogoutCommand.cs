using Common.Blocks.Models.DomainResults;
using MediatR;

namespace Authentication.Application.Features.Authentications.Commands.Logout
{
    public record LogoutCommand : IRequest<Result>
    {
        public required Guid UserId { get; set; }
    }
}
