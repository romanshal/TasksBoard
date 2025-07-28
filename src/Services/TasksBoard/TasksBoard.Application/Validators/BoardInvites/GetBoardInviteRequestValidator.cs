using FluentValidation;
using TasksBoard.Application.Features.BoardInvites.Queries.GetBoardInviteRequestByToAccountId;
using TasksBoard.Domain.Constants.Validations.Messages;

namespace TasksBoard.Application.Validators.BoardInvites
{
    public class GetBoardInviteRequestValidator : AbstractValidator<GetBoardInviteRequestByToAccountIdQuery>
    {
        public GetBoardInviteRequestValidator()
        {
            RuleFor(p => p.AccountId)
                .NotEqual(Guid.Empty)
                .WithMessage(BoardInviteMessages.AccountIdRequired);
        }
    }
}
