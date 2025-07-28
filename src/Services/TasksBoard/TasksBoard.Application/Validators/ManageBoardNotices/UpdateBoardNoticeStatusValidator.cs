using FluentValidation;
using TasksBoard.Application.Features.ManageBoardNotices.Commands.UpdateBoardNoticeStatus;
using TasksBoard.Domain.Constants.Validations.Messages;

namespace TasksBoard.Application.Validators.ManageBoardNotices
{
    public class UpdateBoardNoticeStatusValidator : AbstractValidator<UpdateBoardNoticeStatusCommand>
    {
        public UpdateBoardNoticeStatusValidator()
        {
            RuleFor(p => p.NoticeId)
                .NotEqual(Guid.Empty)
                .WithMessage(BoardNoticeMessages.BoardNoticeIdRequired);

            RuleFor(p => p.BoardId)
                .NotEqual(Guid.Empty)
                .WithMessage(BoardNoticeMessages.BoardIdRequired);

            RuleFor(p => p.AccountId)
                .NotEqual(Guid.Empty)
                .WithMessage(BoardNoticeMessages.AccountIdRequired);

            RuleFor(p => p.AccountName)
                .NotNull()
                .NotEmpty()
                .WithMessage(BoardNoticeMessages.AccountNameReqired);
        }
    }
}
