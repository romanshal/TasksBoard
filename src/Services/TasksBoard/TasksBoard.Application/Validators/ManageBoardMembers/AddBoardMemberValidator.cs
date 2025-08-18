using FluentValidation;
using TasksBoard.Application.Features.ManageBoardMembers.Commands.AddBoardMember;
using TasksBoard.Domain.Constants.Validations.Messages;

namespace TasksBoard.Application.Validators.ManageBoardMembers
{
    public class AddBoardMemberValidator : AbstractValidator<AddBoardMemberCommand>
    {
        public AddBoardMemberValidator()
        {
            RuleFor(p => p.BoardId)
                .NotEqual(Guid.Empty)
                .WithMessage(BoardMemberMessages.BoardIdRequired);

            RuleFor(p => p.AccountId)
                .NotEqual(Guid.Empty)
                .WithMessage(BoardMemberMessages.AccountIdRequired);
        }
    }
}
