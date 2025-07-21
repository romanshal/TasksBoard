using FluentValidation;
using TasksBoard.Application.Features.ManageBoardNotices.Commands.UpdateBoardNotice;
using TasksBoard.Domain.Constants.Messages;

namespace TasksBoard.API.Validators.BoardNotices
{
    public class UpdateBoardNoticeValidator : AbstractValidator<UpdateBoardNoticeCommand>
    {
        public UpdateBoardNoticeValidator()
        {
            RuleFor(x => x.NoticeId)
                .NotNull().NotEmpty().NotEqual(Guid.Empty).WithMessage(BoardNoticeMessages.BoardNoticeIdRequired);

            RuleFor(x => x.Definition)
                .NotNull().NotEmpty().WithMessage(BoardNoticeMessages.DefinitionRequired);
        }
    }
}
