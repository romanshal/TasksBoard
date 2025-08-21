using FluentValidation;
using TasksBoard.Application.Features.BoardAccesses.Commands.RequestBoardAccess;
using TasksBoard.Domain.Constants.Validations.Messages;

namespace TasksBoard.Application.Validators.BoardAccesses
{
    public class RequestBoardAccessValidator : AbstractValidator<RequestBoardAccessCommand>
    {
        public RequestBoardAccessValidator()
        {
            RuleFor(p => p.BoardId)
                .NotEqual(Guid.Empty)
                .WithMessage(BoardAccessMessages.BoardIdRequired);

            RuleFor(p => p.AccountId)
                .NotEqual(Guid.Empty)
                .WithMessage(BoardAccessMessages.AccountIdRequired);
        }
    }
}
