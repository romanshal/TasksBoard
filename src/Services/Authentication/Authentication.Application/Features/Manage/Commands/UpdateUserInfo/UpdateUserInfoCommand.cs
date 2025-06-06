using MediatR;

namespace Authentication.Application.Features.Manage.Commands.UpdateUserInfo
{
    public class UpdateUserInfoCommand : IRequest<Guid>
    {
        public required Guid UserId { get; set; }
        public required string Username { get; set; }
        public required string Email { get; set; }
        public string? Firstname { get; set; }
        public string? Surname { get; set; }
    }
}
