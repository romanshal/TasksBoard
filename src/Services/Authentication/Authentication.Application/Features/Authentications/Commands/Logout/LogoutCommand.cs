using MediatR;

namespace Authentication.Application.Features.Authentications.Commands.Logout
{
    public class LogoutCommand : IRequest<Unit>
    {
        public required Guid UserId { get; set; }
    }
}
