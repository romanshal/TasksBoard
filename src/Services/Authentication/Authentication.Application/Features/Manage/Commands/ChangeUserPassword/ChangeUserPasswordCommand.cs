using Common.Blocks.CQRS;
using Common.Blocks.Models.DomainResults;

namespace Authentication.Application.Features.Manage.Commands.ChangeUserPassword
{
    public class ChangeUserPasswordCommand : ICommand<Result>
    {
        public required Guid UserId { get; set; }
        public required string CurrentPassword { get; set; }
        public required string NewPassword { get; set; }
    }
}
