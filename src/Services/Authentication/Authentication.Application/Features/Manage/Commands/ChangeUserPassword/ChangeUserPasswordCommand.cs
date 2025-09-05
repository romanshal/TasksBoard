using Common.Blocks.Models.DomainResults;
using MediatR;

namespace Authentication.Application.Features.Manage.Commands.ChangeUserPassword
{
    public class ChangeUserPasswordCommand : IRequest<Result>
    {
        public required Guid UserId { get; set; }
        public required string CurrentPassword { get; set; }
        public required string NewPassword { get; set; }
    }
}
