using FluentValidation;
using TasksBoard.Application.Features.BoardAccesses.Queries.GetBoardAccessRequestByAccountId;
using TasksBoard.Domain.Constants.Messages;

namespace TasksBoard.Application.Validators.BoardAccesses
{
    public class GetBoardAccessRequestValidator : AbstractValidator<GetBoardAccessRequestByAccountIdQuery>
    {
        public GetBoardAccessRequestValidator()
        {
            RuleFor(p => p.AccountId)
                .NotEqual(Guid.Empty)
                .WithMessage(BoardAccessMessages.AccountIdRequired);
        }
    }
}
