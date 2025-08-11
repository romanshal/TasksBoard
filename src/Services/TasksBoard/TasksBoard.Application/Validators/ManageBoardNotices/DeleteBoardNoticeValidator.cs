using FluentValidation;
using TasksBoard.Application.Features.ManageBoardNotices.Commands.DeleteBoardNotice;
using TasksBoard.Domain.Constants.Validations.Messages;

namespace TasksBoard.Application.Validators.ManageBoardNotices
{
    public class DeleteBoardNoticeValidator : AbstractValidator<DeleteBoardNoticeCommand>
    {
        public DeleteBoardNoticeValidator()
        {
            RuleFor(p => p.NoticeId)
                .NotEqual(Guid.Empty)
                .WithMessage(BoardNoticeMessages.BoardNoticeIdRequired);

            RuleFor(p => p.BoardId)
                .NotEqual(Guid.Empty)
                .WithMessage(BoardNoticeMessages.BoardIdRequired);
        }
    }
}
