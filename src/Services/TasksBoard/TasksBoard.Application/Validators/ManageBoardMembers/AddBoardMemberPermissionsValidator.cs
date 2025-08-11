using FluentValidation;
using TasksBoard.Application.Features.ManageBoardMembers.Commands.AddBoardMemberPermissions;
using TasksBoard.Domain.Constants.Validations.Messages;

namespace TasksBoard.Application.Validators.ManageBoardMembers
{
    public class AddBoardMemberPermissionsValidator : AbstractValidator<AddBoardMemberPermissionsCommand>
    {
        public AddBoardMemberPermissionsValidator()
        {
            RuleFor(p => p.BoardId)
                .NotEqual(Guid.Empty)
                .WithMessage(BoardMemberMessages.BoardIdRequired);

            RuleFor(p => p.AccountId)
                .NotEqual(Guid.Empty)
                .WithMessage(BoardMemberMessages.AccountIdRequired);

            RuleFor(p => p.MemberId)
                .NotEqual(Guid.Empty)
                .WithMessage(BoardMemberMessages.MemberIdRequired);

            RuleFor(p => p.Permissions)
                .NotNull()
                .NotEmpty()
                .WithMessage(BoardMemberMessages.PermissionsRequired);
        }
    }
}
