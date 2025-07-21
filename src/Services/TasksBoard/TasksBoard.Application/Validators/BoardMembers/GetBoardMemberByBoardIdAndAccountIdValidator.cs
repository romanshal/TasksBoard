using FluentValidation;
using TasksBoard.Application.Features.BoardMembers.Queries.GetBoardMemberByBoardIdAndAccountId;
using TasksBoard.Domain.Constants.Messages;

namespace TasksBoard.Application.Validators.BoardMembers
{
    public class GetBoardMemberByBoardIdAndAccountIdValidator : AbstractValidator<GetBoardMemberByBoardIdAndAccountIdQuery>
    {
        public GetBoardMemberByBoardIdAndAccountIdValidator()
        {
            RuleFor(p => p.AccountId)
                .NotEqual(Guid.Empty)
                .WithMessage(BoardMemberMessages.AccountIdRequired);

            RuleFor(p => p.BoardId)
                .NotEqual(Guid.Empty)
                .WithMessage(BoardMemberMessages.BoardIdRequired);
        }
    }
}
