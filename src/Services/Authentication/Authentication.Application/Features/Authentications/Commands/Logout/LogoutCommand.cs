using MediatR;

namespace Authentication.Application.Features.Authentications.Commands.Logout
{
    public record LogoutCommand : IRequest<Unit>
    {
        public required Guid UserId { get; set; }
    }
}
