using FluentValidation;
using TasksBoard.Application.Features.ManageBoardMembers.Commands.DeleteBoardMember;
using TasksBoard.Domain.Constants.Messages;

namespace TasksBoard.Application.Validators.ManageBoardMembers
{
    public class DeleteBoardMemberValidator : AbstractValidator<DeleteBoardMemberCommand>
    {
        public DeleteBoardMemberValidator()
        {
            RuleFor(p => p.BoardId)
                .NotEqual(Guid.Empty)
                .WithMessage(BoardMemberMessages.BoardIdRequired);

            RuleFor(p => p.MemberId)
                .NotEqual(Guid.Empty)
                .WithMessage(BoardMemberMessages.MemberIdRequired);

            RuleFor(p => p.RemoveByUserId)
                .NotEqual(Guid.Empty)
                .WithMessage(BoardMemberMessages.RemoveAccountIdRequired);
        }
    }
}
