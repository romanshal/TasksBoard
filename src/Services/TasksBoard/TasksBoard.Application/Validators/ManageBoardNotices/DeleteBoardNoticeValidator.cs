using FluentValidation;
using TasksBoard.Application.Features.ManageBoardNotices.Commands.DeleteBoardCommand;
using TasksBoard.Domain.Constants.Messages;

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
