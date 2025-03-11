using FluentValidation;
using TasksBoard.API.Constants.Messages;
using TasksBoard.Application.Features.BoardNotices.Commands.UpdateBoardNotice;

namespace TasksBoard.API.Validators.BoardNotices
{
    public class UpdateBoardNoticeValidator : AbstractValidator<UpdateBoardNoticeCommand>
    {
        public UpdateBoardNoticeValidator()
        {
            RuleFor(x => x.Id)
                .NotNull().NotEmpty().NotEqual(Guid.Empty).WithMessage(BoardNoticeMessages.BoardNoticeIdRequired);

            RuleFor(x => x.NoticeStatusId)
                .NotNull().NotEmpty().NotEqual(Guid.Empty).WithMessage(BoardNoticeMessages.StatusIdRequired);

            RuleFor(x => x.Definition)
                .NotNull().NotEmpty().WithMessage(BoardNoticeMessages.DefinitionRequired);
        }
    }
}
