using FluentValidation;
using TasksBoard.Application.Features.BoardAccesses.Commands.CancelBoardAccess;
using TasksBoard.Domain.Constants.Messages;

namespace TasksBoard.Application.Validators.BoardAccesses
{
    public class CancelBoardAccessValidator : AbstractValidator<CancelBoardAccessCommand>
    {
        public CancelBoardAccessValidator()
        {
            RuleFor(p => p.RequestId)
                .NotEqual(Guid.Empty)
                .WithMessage(BoardAccessMessages.BoardAccessIdRequired);
        }
    }
}
